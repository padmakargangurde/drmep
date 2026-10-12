using Application.Features.ShareLinks.DTOs;
using Application.Features.ShareLinks.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MEP_APIs.Controllers
{
    [Route("api/Project/{projectId:int}/sharelink")]
    [ApiController]
    public class ShareLinkController : ControllerBase
    {

        private readonly IShareLinkService _shareLinkService;

        public ShareLinkController(IShareLinkService shareLinkService)
            => _shareLinkService = shareLinkService;

        [HttpGet]
        [ProducesResponseType(typeof(ShareLinkDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetActive(int projectId, CancellationToken ct)
        {
            var result = await _shareLinkService.GetActiveAsync(projectId, ct);

            return result.Succeeded
                ? Ok(result.Data)
                : NotFound(new { message = result.Error });
        }
         
        [HttpPost]
        [ProducesResponseType(typeof(ShareLinkDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Create(int projectId, [FromQuery] int? createdByUserId, CancellationToken ct)
        {
            var result = await _shareLinkService.CreateAsync(projectId, createdByUserId, ct);

            if (!result.Succeeded)
                return NotFound(new { message = result.Error });

            return CreatedAtAction(nameof(GetActive), new { projectId }, result.Data);
        }

        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Revoke(int projectId, CancellationToken ct)
        {
            var result = await _shareLinkService.RevokeAsync(projectId, ct);

            return result.Succeeded
                ? NoContent()
                : NotFound(new { message = result.Error });
        }
    }
}
