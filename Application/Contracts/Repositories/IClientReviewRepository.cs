using Domain.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Contracts.Repositories
{
    public interface IClientReviewRepository
    {
        Task<ShareLink?> GetActiveShareLinkByTokenAsync(string token, CancellationToken ct = default);

        /// <summary>
        /// Tracked. Null unless the identity belongs to this project AND appears in
        /// the current active revision with a change type other than Removed — a
        /// dropped row can't be responded to.
        /// </summary>
        Task<ItemIdentity?> GetRespondableIdentityAsync(int projectId, int itemIdentityId, CancellationToken ct = default);
    }
}
