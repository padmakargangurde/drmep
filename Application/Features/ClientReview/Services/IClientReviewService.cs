using Application.Common.Results;
using Application.Features.ClientReview.DTOs;
using Application.Features.Revisions.DTOs;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.ClientReview.Services
{
    public interface IClientReviewService
    {
        Task<Result<ClientReviewDto>> GetByTokenAsync(string token, CancellationToken ct = default);

        /// <summary>Finds or creates the AppUser (Role=Client) for this reviewer's email, so comments can carry a real userId.</summary>
        Task<Result<IdentifyClientResult>> IdentifyAsync(string token, IdentifyClientRequest request, CancellationToken ct = default);

        /// <summary>Exports the active revision as an Excel workbook — resolves the project from the token, same as the admin export.</summary>
        Task<Result<ExportedFileDto>> ExportExcelAsync(string token, CancellationToken ct = default);

        Task<Result<ItemResponseDto>> SubmitResponseAsync(
            string token, int itemIdentityId, SubmitResponseRequest request, CancellationToken ct = default);
    }
}
