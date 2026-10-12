using Application.Common.Hashing;
using Application.Common.Results;
using Application.Contracts.Comments;
using Application.Contracts.ExcelFields;
using Application.Contracts.Exporting;
using Application.Contracts.Parsing;
using Application.Contracts.UnitOfWork;
using Application.Features.Revisions.DTOs;
using Application.Features.Revisions.Services;
using Domain.Entities;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Services
{
    public class RevisionService : IRevisionService
    {
        private readonly IUOW _uow;
        private readonly IWorkbookParser _parser;
        private readonly IWorkbookExporter _exporter;
        private readonly IExcelFieldsClient _fieldsClient;
        private readonly ICommentsClient _commentsClient;

        public RevisionService(
            IUOW uow, IWorkbookParser parser, IWorkbookExporter exporter,
            IExcelFieldsClient fieldsClient, ICommentsClient commentsClient)
        {
            _uow = uow;
            _parser = parser;
            _exporter = exporter;
            _fieldsClient = fieldsClient;
            _commentsClient = commentsClient;
        }

        public async Task<Result<UploadRevisionResult>> UploadAsync(
            int projectId, Stream fileStream, string fileName, int? uploadedByUserId, CancellationToken ct = default)
        {
            var project = await _uow.ProjectRepository.GetByIdAsync(projectId, ct);
            if (project is null)
                return Result<UploadRevisionResult>.Failure("Project not found.");

            var parsedCategories = _parser.Parse(fileStream);

            var revisionNumber = await _uow.RevisionRepository.GetNextRevisionNumberAsync(projectId, ct);
            var label = revisionNumber == 1 ? "R01 — Original" : $"R{revisionNumber:D2} — Revision";

            var previousRevision = await _uow.RevisionRepository.GetActiveRevisionForDiffAsync(projectId, ct);
            var oldItems = previousRevision?.Items.ToList() ?? new List<ApprovalItem>();

            // Old rows only carry a FieldsDocumentId in SQL — pull the actual cell values from
            // Mongo so Pass 2's overlap scoring and Pass 3's carry-forward still have Fields to work with.
            var oldDocIds = oldItems
                .Where(o => !string.IsNullOrEmpty(o.FieldsDocumentId))
                .Select(o => o.FieldsDocumentId!)
                .Distinct()
                .ToList();
            var oldFieldsByDocId = await _fieldsClient.GetBatchAsync(oldDocIds, ct);
            foreach (var old in oldItems)
            {
                old.Fields = old.FieldsDocumentId != null && oldFieldsByDocId.TryGetValue(old.FieldsDocumentId, out var f)
                    ? f
                    : new Dictionary<string, string?>();
            }

            var oldByKey = oldItems
                .GroupBy(i => i.ItemIdentity.IdentityKey)
                .ToDictionary(g => g.Key, g => g.First());

            var categoryByName = new Dictionary<string, ApprovalCategory>();
            foreach (var cat in parsedCategories)
            {
                categoryByName[cat.Name] = new ApprovalCategory
                {
                    Name = cat.Name,
                    Columns = cat.Columns,
                    SpecReference = cat.SpecReference,
                    Warnings = cat.Warnings
                };
            }

            var newRows = new List<ApprovalItem>();
            var consumedOldIds = new HashSet<int>();
            var addedCount = 0;
            var modifiedCount = 0;
            var unchangedCount = 0;

            // Pass 1: exact content match — same key means the same row, so it
            // carries its existing ItemIdentity (and therefore its response) forward.
            var remainingNew = new List<(string CategoryName, ParsedItem Item, string Key)>();
            foreach (var cat in parsedCategories.Where(c => !c.Unparsed))
            {
                foreach (var item in cat.Items)
                {
                    var key = IdentityKeyHasher.ComputeKey(cat.Name, item.Section, item.Fields);

                    if (oldByKey.TryGetValue(key, out var oldItem))
                    {
                        unchangedCount++;
                        consumedOldIds.Add(oldItem.ItemIdentityId);
                        var unchangedRow = BuildRow(item, categoryByName[cat.Name], oldItem.ItemIdentity, ItemChangeType.Unchanged, oldItem.Id);
                        unchangedRow.FieldsDocumentId = oldItem.FieldsDocumentId;
                        newRows.Add(unchangedRow);
                    }
                    else
                    {
                        remainingNew.Add((cat.Name, item, key));
                    }
                }
            }

            // Pass 2: best-effort field-overlap match within the same category+section,
            // to tell "modified" apart from a plain add/remove pair.
            var remainingOld = oldItems.Where(o => !consumedOldIds.Contains(o.ItemIdentityId)).ToList();
            var usedOldIds = new HashSet<int>();

            foreach (var (categoryName, item, key) in remainingNew)
            {
                ApprovalItem? best = null;
                var bestScore = -1.0;

                foreach (var old in remainingOld)
                {
                    if (usedOldIds.Contains(old.ItemIdentityId)) continue;
                    if (old.ItemIdentity.CategoryName != categoryName) continue;
                    if ((old.ItemIdentity.Section ?? "") != (item.Section ?? "")) continue;

                    var score = FieldOverlapScore(item.Fields, old.Fields);
                    if (score > bestScore)
                    {
                        bestScore = score;
                        best = old;
                    }
                }

                if (best != null && bestScore >= 0.5)
                {
                    usedOldIds.Add(best.ItemIdentityId);
                    modifiedCount++;
                    var newIdentity = new ItemIdentity
                    {
                        ProjectId = projectId,
                        IdentityKey = key,
                        CategoryName = categoryName,
                        Section = item.Section
                    };
                    newRows.Add(BuildRow(item, categoryByName[categoryName], newIdentity, ItemChangeType.Modified, best.Id));
                }
                else
                {
                    addedCount++;
                    var newIdentity = new ItemIdentity
                    {
                        ProjectId = projectId,
                        IdentityKey = key,
                        CategoryName = categoryName,
                        Section = item.Section
                    };
                    newRows.Add(BuildRow(item, categoryByName[categoryName], newIdentity, ItemChangeType.Added, null));
                }
            }

            // Pass 3: whatever's left in the old revision was dropped from this upload.
            // Carried into the new revision as a Removed row so its history stays visible.
            var removedOld = oldItems
                .Where(o => !consumedOldIds.Contains(o.ItemIdentityId) && !usedOldIds.Contains(o.ItemIdentityId))
                .ToList();
            var removedCount = removedOld.Count;

            foreach (var old in removedOld)
            {
                var categoryName = old.ItemIdentity.CategoryName;
                if (!categoryByName.TryGetValue(categoryName, out var category))
                {
                    category = new ApprovalCategory
                    {
                        Name = categoryName,
                        Columns = old.Fields.Keys.ToList(),
                        SpecReference = old.SpecReference,
                        Warnings = { "This category was present in a previous revision but is missing from this upload." }
                    };
                    categoryByName[categoryName] = category;
                }

                var removedItem = new ApprovalItem
                {
                    ApprovalCategory = category,
                    ItemIdentity = old.ItemIdentity,
                    ItemNo = old.ItemNo,
                    Section = old.Section,
                    Fields = old.Fields,
                    FieldsDocumentId = old.FieldsDocumentId,
                    SourceStatus = old.SourceStatus,
                    SpecReference = old.SpecReference,
                    LowConfidence = old.LowConfidence,
                    ChangeType = ItemChangeType.Removed,
                    PreviousItemId = old.Id
                };
                newRows.Add(removedItem);
            }

            // Added/Modified rows always carry genuinely new cell content. Unchanged/Removed rows
            // only skip Mongo if they already have a document to carry forward.
            var rowsNeedingNewFields = newRows
                .Where(r => string.IsNullOrEmpty(r.FieldsDocumentId))
                .ToList();
            if (rowsNeedingNewFields.Count > 0)
            {
                var saveRequests = rowsNeedingNewFields.Select(r => new ExcelFieldsRow
                {
                    ProjectId = project.Id,
                    ProjectNumber = project.ProjectNumber,
                    RevisionLabel = label,
                    CategoryName = r.ApprovalCategory.Name,
                    Section = r.Section,
                    ItemNo = r.ItemNo,
                    Fields = r.Fields
                }).ToList();

                var savedIds = await _fieldsClient.SaveBatchAsync(saveRequests, ct);
                for (var i = 0; i < rowsNeedingNewFields.Count; i++)
                    rowsNeedingNewFields[i].FieldsDocumentId = savedIds[i];
            }

            // A client-supplied uploadedByUserId that doesn't exist must not be written anywhere as
            // a foreign key — resolve it once up front and reuse the validated value everywhere.
            var actorName = "System";
            var validatedActorUserId = (int?)null;
            if (uploadedByUserId.HasValue)
            {
                var uploader = await _uow.UserRepository.GetByIdAsync(uploadedByUserId.Value, ct);
                if (uploader != null)
                {
                    actorName = uploader.Name;
                    validatedActorUserId = uploadedByUserId;
                }
            }

            var revision = new ProjectRevision
            {
                ProjectId = projectId,
                RevisionNumber = revisionNumber,
                Label = label,
                SourceFileName = fileName,
                UploadedAt = DateTime.UtcNow,
                UploadedByUserId = validatedActorUserId,
                IsActive = true,
                Categories = categoryByName.Values.ToList(),
                Items = newRows
            };

            if (previousRevision != null)
                previousRevision.IsActive = false;

            project.IsPublished = true;

            var auditEntry = new AuditLogEntry
            {
                ProjectId = projectId,
                Timestamp = DateTime.UtcNow,
                ActorUserId = validatedActorUserId,
                ActorDisplayName = actorName,
                Action = "Revision published",
                Detail = $"{label} — {addedCount} added, {modifiedCount} modified, {removedCount} removed, {unchangedCount} carried forward."
            };

            await _uow.BeginTransactionAsync(ct);
            try
            {
                await _uow.RevisionRepository.AddRevisionAsync(revision, ct);
                await _uow.AuditLogRepository.AddAsync(auditEntry, ct);
                await _uow.SaveChangesAsync(ct);
                await _uow.CommitTransactionAsync(ct);
            }
            catch
            {
                await _uow.RollbackTransactionAsync(ct);
                throw;
            }

            return Result<UploadRevisionResult>.Success(new UploadRevisionResult
            {
                RevisionId = revision.Id,
                RevisionNumber = revisionNumber,
                Label = label,
                CategoryCount = categoryByName.Count,
                TotalItems = newRows.Count,
                AddedCount = addedCount,
                ModifiedCount = modifiedCount,
                RemovedCount = removedCount,
                UnchangedCount = unchangedCount,
                Warnings = parsedCategories.SelectMany(c => c.Warnings).ToList()
            });
        }

        public async Task<Result<IReadOnlyList<RevisionListItemDto>>> GetAllAsync(int projectId, CancellationToken ct = default)
        {
            var project = await _uow.ProjectRepository.GetByIdAsync(projectId, ct);
            if (project is null)
                return Result<IReadOnlyList<RevisionListItemDto>>.Failure("Project not found.");

            var revisions = await _uow.RevisionRepository.GetAllForProjectAsync(projectId, ct);

            IReadOnlyList<RevisionListItemDto> dtos = revisions.Select(r => new RevisionListItemDto
            {
                RevisionId = r.Id,
                RevisionNumber = r.RevisionNumber,
                Label = r.Label,
                UploadedAt = r.UploadedAt,
                UploadedByName = r.UploadedBy?.Name,
                IsActive = r.IsActive,
                ItemCount = r.Items.Count
            }).ToList();

            return Result<IReadOnlyList<RevisionListItemDto>>.Success(dtos);
        }

        public async Task<Result<RevisionDetailDto>> GetActiveAsync(int projectId, CancellationToken ct = default)
        {
            var active = await _uow.RevisionRepository.GetActiveRevisionForDiffAsync(projectId, ct);
            if (active is null)
                return Result<RevisionDetailDto>.Failure("This project has no active revision yet.");

            return await GetByIdAsync(projectId, active.Id, ct);
        }

        public async Task<Result<RevisionDetailDto>> GetByIdAsync(int projectId, int revisionId, CancellationToken ct = default)
        {
            var revision = await _uow.RevisionRepository.GetByIdWithDetailsAsync(revisionId, ct);
            if (revision is null || revision.ProjectId != projectId)
                return Result<RevisionDetailDto>.Failure("Revision not found.");

            var docIds = revision.Items
                .Where(i => !string.IsNullOrEmpty(i.FieldsDocumentId))
                .Select(i => i.FieldsDocumentId!)
                .Distinct()
                .ToList();
            var fieldsByDocId = await _fieldsClient.GetBatchAsync(docIds, ct);

            var itemIdentityIds = revision.Items.Select(i => i.ItemIdentityId).Distinct().ToList();
            var commentsByIdentityId = await _commentsClient.GetCommentsBatchAsync(itemIdentityIds, ct);

            return Result<RevisionDetailDto>.Success(new RevisionDetailDto
            {
                RevisionId = revision.Id,
                RevisionNumber = revision.RevisionNumber,
                Label = revision.Label,
                IsActive = revision.IsActive,
                Categories = revision.Categories.Select(c => new CategoryDto
                {
                    CategoryId = c.Id,
                    Name = c.Name,
                    Columns = c.Columns,
                    SpecReference = c.SpecReference,
                    Warnings = c.Warnings
                }).ToList(),
                Items = revision.Items.Select(i => new ApprovalItemDto
                {
                    ItemId = i.Id,
                    ItemIdentityId = i.ItemIdentityId,
                    CategoryName = i.ItemIdentity.CategoryName,
                    Section = i.Section,
                    ItemNo = i.ItemNo,
                    Fields = i.FieldsDocumentId != null && fieldsByDocId.TryGetValue(i.FieldsDocumentId, out var itemFields)
                        ? itemFields
                        : new Dictionary<string, string?>(),
                    SourceStatus = i.SourceStatus,
                    SpecReference = i.SpecReference,
                    LowConfidence = i.LowConfidence,
                    ChangeType = i.ChangeType,
                    Decision = i.ItemIdentity.Response?.Decision ?? ReviewDecision.Pending,
                    Comment = i.ItemIdentity.Response?.Comment,
                    ReviewedByName = i.ItemIdentity.Response?.ReviewedByName,
                    RespondedAt = i.ItemIdentity.Response?.RespondedAt,
                    Comments = commentsByIdentityId.TryGetValue(i.ItemIdentityId, out var itemComments)
                        ? itemComments.Select(c => new CommentEntryDto
                        {
                            UserId = c.UserId,
                            Email = c.Email,
                            Comment = c.Comment,
                            ReviewedByName = c.ReviewedByName,
                            Role = c.Role,
                            CreatedAt = c.CreatedAt
                        }).ToList()
                        : new List<CommentEntryDto>()
                }).ToList()
            });
        }

        public async Task<Result<ExportedFileDto>> ExportActiveAsync(int projectId, CancellationToken ct = default)
        {
            var project = await _uow.ProjectRepository.GetByIdAsync(projectId, ct);
            if (project is null)
                return Result<ExportedFileDto>.Failure("Project not found.");

            var revisionResult = await GetActiveAsync(projectId, ct);
            if (!revisionResult.Succeeded)
                return Result<ExportedFileDto>.Failure(revisionResult.Error!);

            var revision = revisionResult.Data!;
            var content = _exporter.ExportApprovalMatrix(new ExportWorkbookRequest
            {
                ProjectNumber = project.ProjectNumber,
                ProjectName = project.Name,
                ClientName = project.ClientName,
                RevisionLabel = revision.Label,
                Categories = revision.Categories,
                Items = revision.Items
            });

            var fileName = $"{project.ProjectNumber}_{revision.Label.Replace(" ", "")}_Approval.xlsx";
            return Result<ExportedFileDto>.Success(new ExportedFileDto { Content = content, FileName = fileName });
        }

        private static ApprovalItem BuildRow(
            ParsedItem item, ApprovalCategory category, ItemIdentity identity, ItemChangeType changeType, int? previousItemId)
        {
            return new ApprovalItem
            {
                ApprovalCategory = category,
                ItemIdentity = identity,
                ItemNo = item.ItemNo,
                Section = item.Section,
                Fields = item.Fields,
                SourceStatus = item.SourceStatus,
                SpecReference = item.SpecReference,
                LowConfidence = item.LowConfidence,
                ChangeType = changeType,
                PreviousItemId = previousItemId
            };
        }

        private static double FieldOverlapScore(Dictionary<string, string?> a, Dictionary<string, string?> b)
        {
            var allKeys = a.Keys.Union(b.Keys).ToList();
            if (allKeys.Count == 0) return 0;

            var match = allKeys.Count(k =>
                (a.GetValueOrDefault(k) ?? "").Trim().ToLowerInvariant()
                == (b.GetValueOrDefault(k) ?? "").Trim().ToLowerInvariant());

            return (double)match / allKeys.Count;
        }
    }
}
