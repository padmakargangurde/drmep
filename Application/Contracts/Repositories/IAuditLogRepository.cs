using Domain.Entities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Contracts.Repositories
{
    public interface IAuditLogRepository
    {
        Task AddAsync(AuditLogEntry entry, CancellationToken ct = default);

        Task<IReadOnlyList<AuditLogEntry>> GetForProjectAsync(int projectId, CancellationToken ct = default);
    }
}
