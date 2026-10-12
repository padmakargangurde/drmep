using Application.Common.Results;
using Application.Features.ShareLinks.DTOs;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.ShareLinks.Services
{
    public interface IShareLinkService
    {
        Task<Result<ShareLinkDto>> GetActiveAsync(int projectId, CancellationToken ct = default);

        Task<Result<ShareLinkDto>> CreateAsync(int projectId, int? createdByUserId, CancellationToken ct = default);

        Task<Result<bool>> RevokeAsync(int projectId, CancellationToken ct = default);
    }
}
