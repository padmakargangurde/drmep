using Domain.Entities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Contracts.Repositories
{
    public interface IRevisionRepository
    {
        Task<int> GetNextRevisionNumberAsync(int projectId, CancellationToken ct = default);

        /// <summary>
        /// The project's current active revision, tracked (not AsNoTracking) — its
        /// items and identities are reused by reference when building the next
        /// revision's Unchanged/Removed rows, so EF must recognize them as already
        /// persisted rather than re-insert them.
        /// </summary>
        Task<ProjectRevision?> GetActiveRevisionForDiffAsync(int projectId, CancellationToken ct = default);

        Task AddRevisionAsync(ProjectRevision revision, CancellationToken ct = default);

        Task<IReadOnlyList<ProjectRevision>> GetAllForProjectAsync(int projectId, CancellationToken ct = default);

        Task<ProjectRevision?> GetByIdWithDetailsAsync(int revisionId, CancellationToken ct = default);
    }
}
