using Application.Common.ReviewStats;
using Application.Common.Results;
using Application.Contracts.UnitOfWork;
using Application.Features.FinalSubmission.DTOs;
using Application.Features.FinalSubmission.Services;
using Application.Features.Revisions.Services;
using Domain.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;
using FinalSubmissionEntity = Domain.Entities.FinalSubmission;

namespace Application.Services
{
    public class FinalSubmissionService : IFinalSubmissionService
    {
        private readonly IUOW _uow;
        private readonly IRevisionService _revisionService;

        public FinalSubmissionService(IUOW uow, IRevisionService revisionService)
        {
            _uow = uow;
            _revisionService = revisionService;
        }

        public async Task<Result<FinalSubmissionStatusDto>> GetStatusAsync(int projectId, CancellationToken ct = default)
        {
            var project = await _uow.ProjectRepository.GetByIdAsync(projectId, ct);
            if (project is null)
                return Result<FinalSubmissionStatusDto>.Failure("Project not found.");

            var latest = await _uow.FinalSubmissionRepository.GetLatestByProjectAsync(projectId, ct);
            return Result<FinalSubmissionStatusDto>.Success(ToStatusDto(latest));
        }

        public async Task<Result<FinalSubmissionStatusDto>> SubmitAsync(
            string token, SubmitFinalRequest request, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(request.SubmittedByName))
                return Result<FinalSubmissionStatusDto>.Failure("Name is required.");

            var link = await _uow.ClientReviewRepository.GetActiveShareLinkByTokenAsync(token, ct);
            if (link is null)
                return Result<FinalSubmissionStatusDto>.Failure("This link is invalid or has been revoked.");

            var latest = await _uow.FinalSubmissionRepository.GetLatestByProjectAsync(link.ProjectId, ct);
            if (latest != null && latest.ReopenedAt is null)
                return Result<FinalSubmissionStatusDto>.Failure("This approval has already been submitted and is locked.");

            var activeRevision = await _revisionService.GetActiveAsync(link.ProjectId, ct);
            if (!activeRevision.Succeeded)
                return Result<FinalSubmissionStatusDto>.Failure(activeRevision.Error!);

            var detail = activeRevision.Data!;
            var stats = ReviewStatsCalculator.Compute(detail.Items);
            var now = DateTime.UtcNow;

            var submission = new FinalSubmissionEntity
            {
                ProjectId = link.ProjectId,
                ProjectRevisionId = detail.RevisionId,
                SubmittedByName = request.SubmittedByName.Trim(),
                SubmittedByCompany = request.SubmittedByCompany?.Trim(),
                SubmittedAt = now,
                ApprovedCount = stats.Approved,
                RejectedCount = stats.Rejected,
                ClarifyCount = stats.Clarify,
                NaCount = stats.Na,
                CommentedCount = stats.Commented,
                CreatedAt = now
            };

            await _uow.FinalSubmissionRepository.AddAsync(submission, ct);

            var auditEntry = new AuditLogEntry
            {
                ProjectId = link.ProjectId,
                Timestamp = now,
                ActorUserId = null,
                ActorDisplayName = submission.SubmittedByName,
                Action = "Final approval submitted",
                Detail = detail.Label
            };
            await _uow.AuditLogRepository.AddAsync(auditEntry, ct);

            await _uow.SaveChangesAsync(ct);

            return Result<FinalSubmissionStatusDto>.Success(ToStatusDto(submission));
        }

        public async Task<Result<FinalSubmissionStatusDto>> ReopenAsync(
            int projectId, int? reopenedByUserId, CancellationToken ct = default)
        {
            var latest = await _uow.FinalSubmissionRepository.GetLatestByProjectAsync(projectId, ct);
            if (latest is null || latest.ReopenedAt != null)
                return Result<FinalSubmissionStatusDto>.Failure("This project is not currently locked.");

            var now = DateTime.UtcNow;
            latest.ReopenedAt = now;
            latest.ReopenedByUserId = reopenedByUserId;

            var actorName = "Admin";
            if (reopenedByUserId.HasValue)
            {
                var user = await _uow.UserRepository.GetByIdAsync(reopenedByUserId.Value, ct);
                if (user != null) actorName = user.Name;
            }

            var auditEntry = new AuditLogEntry
            {
                ProjectId = projectId,
                Timestamp = now,
                ActorUserId = reopenedByUserId,
                ActorDisplayName = actorName,
                Action = "Approval reopened",
                Detail = "Client review re-enabled."
            };
            await _uow.AuditLogRepository.AddAsync(auditEntry, ct);

            await _uow.SaveChangesAsync(ct);

            return Result<FinalSubmissionStatusDto>.Success(ToStatusDto(latest));
        }

        private static FinalSubmissionStatusDto ToStatusDto(FinalSubmissionEntity? latest)
        {
            if (latest is null)
                return new FinalSubmissionStatusDto { IsLocked = false };

            return new FinalSubmissionStatusDto
            {
                IsLocked = latest.ReopenedAt is null,
                SubmittedByName = latest.SubmittedByName,
                SubmittedByCompany = latest.SubmittedByCompany,
                SubmittedAt = latest.SubmittedAt,
                ApprovedCount = latest.ApprovedCount,
                RejectedCount = latest.RejectedCount,
                ClarifyCount = latest.ClarifyCount,
                NaCount = latest.NaCount,
                CommentedCount = latest.CommentedCount,
                ReopenedAt = latest.ReopenedAt
            };
        }
    }
}
