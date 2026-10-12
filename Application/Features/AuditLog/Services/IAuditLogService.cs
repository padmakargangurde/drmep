using Application.Common.Results;
using Application.Features.AuditLog.DTOs;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.AuditLog.Services
{
    public interface IAuditLogService
    {
        Task<Result<IReadOnlyList<AuditLogEntryDto>>> GetForProjectAsync(int projectId, CancellationToken ct = default);
    }
}
