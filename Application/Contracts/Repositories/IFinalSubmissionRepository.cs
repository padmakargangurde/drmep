using Domain.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Contracts.Repositories
{
    public interface IFinalSubmissionRepository
    {
        /// <summary>Tracked. Null if the project has never been submitted.</summary>
        Task<FinalSubmission?> GetLatestByProjectAsync(int projectId, CancellationToken ct = default);

        Task AddAsync(FinalSubmission submission, CancellationToken ct = default);
    }
}
