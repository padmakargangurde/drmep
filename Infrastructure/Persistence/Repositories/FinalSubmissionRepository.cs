using Application.Contracts.Repositories;
using Domain.Entities;
using Infrastructure.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Repositories
{
    public class FinalSubmissionRepository : IFinalSubmissionRepository
    {
        private readonly AppDbContext _context;

        public FinalSubmissionRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<FinalSubmission?> GetLatestByProjectAsync(int projectId, CancellationToken ct = default)
        {
            return await _context.FinalSubmissions
                .Where(s => s.ProjectId == projectId)
                .OrderByDescending(s => s.SubmittedAt)
                .FirstOrDefaultAsync(ct);
        }

        public async Task AddAsync(FinalSubmission submission, CancellationToken ct = default)
        {
            await _context.FinalSubmissions.AddAsync(submission, ct);
        }
    }
}
