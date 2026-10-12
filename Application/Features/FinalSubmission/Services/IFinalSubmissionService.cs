using Application.Common.Results;
using Application.Features.FinalSubmission.DTOs;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.FinalSubmission.Services
{
    public interface IFinalSubmissionService
    {
        Task<Result<FinalSubmissionStatusDto>> GetStatusAsync(int projectId, CancellationToken ct = default);

        Task<Result<FinalSubmissionStatusDto>> SubmitAsync(
            string token, SubmitFinalRequest request, CancellationToken ct = default);

        Task<Result<FinalSubmissionStatusDto>> ReopenAsync(
            int projectId, int? reopenedByUserId, CancellationToken ct = default);
    }
}
