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
    public class RevisionRepository : IRevisionRepository
    {
        private readonly AppDbContext _context;

        public RevisionRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<int> GetNextRevisionNumberAsync(int projectId, CancellationToken ct = default)
        {
            var maxRevisionNumber = await _context.ProjectRevisions
                .Where(r => r.ProjectId == projectId)
                .Select(r => (int?)r.RevisionNumber)
                .MaxAsync(ct);

            return (maxRevisionNumber ?? 0) + 1;
        }

        public async Task<ProjectRevision?> GetActiveRevisionForDiffAsync(int projectId, CancellationToken ct = default)
        {
            return await _context.ProjectRevisions
                .Include(r => r.Items)
                    .ThenInclude(i => i.ItemIdentity)
                .FirstOrDefaultAsync(r => r.ProjectId == projectId && r.IsActive, ct);
        }

        public async Task AddRevisionAsync(ProjectRevision revision, CancellationToken ct = default)
        {
            await _context.ProjectRevisions.AddAsync(revision, ct);
        }

        public async Task<IReadOnlyList<ProjectRevision>> GetAllForProjectAsync(int projectId, CancellationToken ct = default)
        {
            return await _context.ProjectRevisions
                .Include(r => r.UploadedBy)
                .Include(r => r.Items)
                .Where(r => r.ProjectId == projectId)
                .AsNoTracking()
                .OrderByDescending(r => r.RevisionNumber)
                .ToListAsync(ct);
        }

        public async Task<ProjectRevision?> GetByIdWithDetailsAsync(int revisionId, CancellationToken ct = default)
        {
            return await _context.ProjectRevisions
                .Include(r => r.Categories)
                .Include(r => r.Items)
                    .ThenInclude(i => i.ItemIdentity)
                        .ThenInclude(ii => ii.Response)
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == revisionId, ct);
        }
    }
}
