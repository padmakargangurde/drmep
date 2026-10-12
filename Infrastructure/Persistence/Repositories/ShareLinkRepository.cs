using Application.Contracts.Repositories;
using Domain.Entities;
using Infrastructure.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Repositories
{
    public class ShareLinkRepository : IShareLinkRepository
    {
        private readonly AppDbContext _context;

        public ShareLinkRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ShareLink?> GetActiveByProjectAsync(int projectId, CancellationToken ct = default)
        {
            return await _context.ShareLinks
                .FirstOrDefaultAsync(s => s.ProjectId == projectId && !s.IsRevoked, ct);
        }

        public async Task AddAsync(ShareLink link, CancellationToken ct = default)
        {
            await _context.ShareLinks.AddAsync(link, ct);
        }
    }
}
