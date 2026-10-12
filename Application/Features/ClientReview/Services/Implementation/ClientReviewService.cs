using Application.Common.ReviewStats;
using Application.Common.Results;
using Application.Contracts.Comments;
using Application.Contracts.UnitOfWork;
using Application.Features.ClientReview.DTOs;
using Application.Features.ClientReview.Services;
using Application.Features.FinalSubmission.Services;
using Application.Features.Revisions.DTOs;
using Application.Features.Revisions.Services;
using Domain.Entities;
using Domain.Enums;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Services
{
    public class ClientReviewService : IClientReviewService
    {
        private readonly IUOW _uow;
        private readonly IRevisionService _revisionService;
        private readonly IFinalSubmissionService _finalSubmissionService;
        private readonly ICommentsClient _commentsClient;

        public ClientReviewService(
            IUOW uow, IRevisionService revisionService, IFinalSubmissionService finalSubmissionService,
            ICommentsClient commentsClient)
        {
            _uow = uow;
            _revisionService = revisionService;
            _finalSubmissionService = finalSubmissionService;
            _commentsClient = commentsClient;
        }

        public async Task<Result<ClientReviewDto>> GetByTokenAsync(string token, CancellationToken ct = default)
        {
            var link = await _uow.ClientReviewRepository.GetActiveShareLinkByTokenAsync(token, ct);
            if (link is null)
                return Result<ClientReviewDto>.Failure("This link is invalid or has been revoked.");

            var activeRevision = await _revisionService.GetActiveAsync(link.ProjectId, ct);
            if (!activeRevision.Succeeded)
                return Result<ClientReviewDto>.Failure(activeRevision.Error!);

            var detail = activeRevision.Data!;
            var statusResult = await _finalSubmissionService.GetStatusAsync(link.ProjectId, ct);

            return Result<ClientReviewDto>.Success(new ClientReviewDto
            {
                ProjectNumber = link.Project.ProjectNumber,
                Name = link.Project.Name,
                ClientName = link.Project.ClientName,
                RevisionLabel = detail.Label,
                ReviewStats = ReviewStatsCalculator.Compute(detail.Items),
                FinalStatus = statusResult.Data ?? new() { IsLocked = false },
                Categories = detail.Categories,
                Items = detail.Items
            });
        }

        public async Task<Result<ExportedFileDto>> ExportExcelAsync(string token, CancellationToken ct = default)
        {
            var link = await _uow.ClientReviewRepository.GetActiveShareLinkByTokenAsync(token, ct);
            if (link is null)
                return Result<ExportedFileDto>.Failure("This link is invalid or has been revoked.");

            return await _revisionService.ExportActiveAsync(link.ProjectId, ct);
        }

        public async Task<Result<IdentifyClientResult>> IdentifyAsync(
            string token, IdentifyClientRequest request, CancellationToken ct = default)
        {
            var link = await _uow.ClientReviewRepository.GetActiveShareLinkByTokenAsync(token, ct);
            if (link is null)
                return Result<IdentifyClientResult>.Failure("This link is invalid or has been revoked.");

            var name = request.Name.Trim();
            var email = request.Email.Trim().ToLowerInvariant();

            var user = await _uow.UserRepository.GetByEmailAsync(email, ct);
            if (user is null)
            {
                user = new AppUser
                {
                    Name = name,
                    Email = email,
                    Role = UserRole.Client,
                    IsActive = true
                };
                await _uow.UserRepository.AddAsync(user, ct);
                await _uow.SaveChangesAsync(ct);
            }
            else if (user.Name != name)
            {
                // Same reviewer, updated display name — keep the identity, refresh what they're called.
                user.Name = name;
                await _uow.UserRepository.UpdateAsync(user, ct);
                await _uow.SaveChangesAsync(ct);
            }

            return Result<IdentifyClientResult>.Success(new IdentifyClientResult
            {
                UserId = user.Id,
                Name = user.Name,
                Email = user.Email
            });
        }

        public async Task<Result<ItemResponseDto>> SubmitResponseAsync(
            string token, int itemIdentityId, SubmitResponseRequest request, CancellationToken ct = default)
        {
            var link = await _uow.ClientReviewRepository.GetActiveShareLinkByTokenAsync(token, ct);
            if (link is null)
                return Result<ItemResponseDto>.Failure("This link is invalid or has been revoked.");

            var statusResult = await _finalSubmissionService.GetStatusAsync(link.ProjectId, ct);
            if (statusResult.Data?.IsLocked == true)
                return Result<ItemResponseDto>.Failure("This approval has been submitted and is locked.");

            var identity = await _uow.ClientReviewRepository.GetRespondableIdentityAsync(link.ProjectId, itemIdentityId, ct);
            if (identity is null)
                return Result<ItemResponseDto>.Failure("Item not found.");

            var reviewerName = string.IsNullOrWhiteSpace(request.ReviewedByName)
                ? "Client"
                : request.ReviewedByName.Trim();
            var now = DateTime.UtcNow;

            if (identity.Response is null)
            {
                identity.Response = new ItemResponse
                {
                    ItemIdentityId = identity.Id,
                    Decision = request.Decision,
                    Comment = request.Comment,
                    ReviewedByName = reviewerName,
                    RespondedAt = now,
                    CreatedAt = now
                };
            }
            else
            {
                identity.Response.Decision = request.Decision;
                identity.Response.Comment = request.Comment;
                identity.Response.ReviewedByName = reviewerName;
                identity.Response.RespondedAt = now;
                identity.Response.UpdatedAt = now;
            }

            var auditEntry = new AuditLogEntry
            {
                ProjectId = link.ProjectId,
                Timestamp = now,
                ActorUserId = null,
                ActorDisplayName = reviewerName,
                Action = "Response recorded",
                Detail = $"{identity.CategoryName} — {DecisionLabel(request.Decision)}"
            };
            await _uow.AuditLogRepository.AddAsync(auditEntry, ct);

            await _uow.SaveChangesAsync(ct);

            // Comments accumulate across reviewers/re-submissions — append rather than overwrite,
            // unlike Decision above which is the single current status.
            if (!string.IsNullOrWhiteSpace(request.Comment))
            {
                await _commentsClient.AddCommentAsync(
                    identity.Id, request.Comment!, reviewerName, role: "Client",
                    userId: request.UserId, email: request.Email, ct: ct);
            }

            return Result<ItemResponseDto>.Success(new ItemResponseDto
            {
                ItemIdentityId = identity.Id,
                Decision = identity.Response.Decision,
                Comment = identity.Response.Comment,
                ReviewedByName = identity.Response.ReviewedByName,
                RespondedAt = identity.Response.RespondedAt
            });
        }

        private static string DecisionLabel(ReviewDecision decision) => decision switch
        {
            ReviewDecision.Pending => "Pending",
            ReviewDecision.Approved => "Approved",
            ReviewDecision.Rejected => "Rejected",
            ReviewDecision.Commented => "Approved w/ Comments",
            ReviewDecision.Clarify => "Need Clarification",
            ReviewDecision.NA => "Not Applicable",
            _ => decision.ToString()
        };
    }
}
