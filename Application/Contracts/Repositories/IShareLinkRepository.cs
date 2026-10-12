using Domain.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Contracts.Repositories
{
    public interface IShareLinkRepository
    {
        /// <summary>Tracked — the caller may revoke it in the same unit of work.</summary>
        Task<ShareLink?> GetActiveByProjectAsync(int projectId, CancellationToken ct = default);

        Task AddAsync(ShareLink link, CancellationToken ct = default);
    }
}
