using Application.Common.Results;
using Application.Contracts.UnitOfWork;
using Application.Features.AuditLog.DTOs;
using Application.Features.AuditLog.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Services
{
    public class AuditLogService : IAuditLogService
    {
        private readonly IUOW _uow;

        public AuditLogService(IUOW uow) => _uow = uow;

        public async Task<Result<IReadOnlyList<AuditLogEntryDto>>> GetForProjectAsync(int projectId, CancellationToken ct = default)
        {
            var project = await _uow.ProjectRepository.GetByIdAsync(projectId, ct);
            if (project is null)
                return Result<IReadOnlyList<AuditLogEntryDto>>.Failure("Project not found.");

            var entries = await _uow.AuditLogRepository.GetForProjectAsync(projectId, ct);

            IReadOnlyList<AuditLogEntryDto> dtos = entries.Select(e => new AuditLogEntryDto
            {
                Timestamp = e.Timestamp,
                ActorDisplayName = e.ActorDisplayName,
                Action = e.Action,
                Detail = e.Detail
            }).ToList();

            return Result<IReadOnlyList<AuditLogEntryDto>>.Success(dtos);
        }
    }
}
