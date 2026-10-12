using Application.Contracts.Repositories;
using Domain.Entities;
using Infrastructure.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Repositories
{
    public class AuditLogRepository : IAuditLogRepository
    {
        private readonly AppDbContext _context;

        public AuditLogRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(AuditLogEntry entry, CancellationToken ct = default)
        {
            await _context.AuditLogEntries.AddAsync(entry, ct);
        }

        public async Task<IReadOnlyList<AuditLogEntry>> GetForProjectAsync(int projectId, CancellationToken ct = default)
        {
            return await _context.AuditLogEntries
                .Where(a => a.ProjectId == projectId)
                .AsNoTracking()
                .OrderByDescending(a => a.Timestamp)
                .ToListAsync(ct);
        }
    }
}
