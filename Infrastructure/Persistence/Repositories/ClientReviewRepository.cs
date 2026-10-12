using Application.Contracts.Repositories;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Repositories
{
    public class ClientReviewRepository : IClientReviewRepository
    {
        private readonly AppDbContext _context;

        public ClientReviewRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ShareLink?> GetActiveShareLinkByTokenAsync(string token, CancellationToken ct = default)
        {
            return await _context.ShareLinks
                .Include(s => s.Project)
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Token == token && !s.IsRevoked, ct);
        }

        public async Task<ItemIdentity?> GetRespondableIdentityAsync(int projectId, int itemIdentityId, CancellationToken ct = default)
        {
            var identity = await _context.ItemIdentities
                .Include(ii => ii.Response)
                .FirstOrDefaultAsync(ii => ii.Id == itemIdentityId && ii.ProjectId == projectId, ct);

            if (identity is null) return null;

            var isRespondable = await _context.ApprovalItems.AnyAsync(
                i => i.ItemIdentityId == itemIdentityId
                    && i.ProjectRevision.IsActive
                    && i.ChangeType != ItemChangeType.Removed,
                ct);

            return isRespondable ? identity : null;
        }
    }
}
