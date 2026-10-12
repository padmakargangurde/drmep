using Application.Features.ClientReview.DTOs;
using Application.Features.ClientReview.Services;
using Application.Features.FinalSubmission.DTOs;
using Application.Features.FinalSubmission.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MEP_APIs.Controllers
{
    /// <summary>
    /// Public, unauthenticated surface — the token in the route is the only
    /// credential a client has. No project id appears in the route, so a bad
    /// guess can't be used to enumerate projects.
    /// </summary>
    [Route("api/ClientReview/{token}")]
    [ApiController]
    public class ClientReviewController : ControllerBase
    {
        private readonly IClientReviewService _clientReviewService;
        private readonly IFinalSubmissionService _finalSubmissionService;

        public ClientReviewController(
            IClientReviewService clientReviewService, IFinalSubmissionService finalSubmissionService)
        {
            _clientReviewService = clientReviewService;
            _finalSubmissionService = finalSubmissionService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ClientReviewDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get(string token, CancellationToken ct)
        {
            var result = await _clientReviewService.GetByTokenAsync(token, ct);

            return result.Succeeded
                ? Ok(result.Data)
                : NotFound(new { message = result.Error });
        }

        [HttpGet("export")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Export(string token, CancellationToken ct)
        {
            var result = await _clientReviewService.ExportExcelAsync(token, ct);

            if (!result.Succeeded)
                return NotFound(new { message = result.Error });

            return File(result.Data!.Content, result.Data.ContentType, result.Data.FileName);
        }

        [HttpPost("identify")]
        [ProducesResponseType(typeof(IdentifyClientResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Identify(
            string token, [FromBody] IdentifyClientRequest request, CancellationToken ct)
        {
            var result = await _clientReviewService.IdentifyAsync(token, request, ct);

            return result.Succeeded
                ? Ok(result.Data)
                : NotFound(new { message = result.Error });
        }

        [HttpPut("items/{itemIdentityId:int}")]
        [ProducesResponseType(typeof(ItemResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> SubmitResponse(
            string token, int itemIdentityId, [FromBody] SubmitResponseRequest request, CancellationToken ct)
        {
            var result = await _clientReviewService.SubmitResponseAsync(token, itemIdentityId, request, ct);

            if (result.Succeeded)
                return Ok(result.Data);

            return result.Error == "This approval has been submitted and is locked."
                ? Conflict(new { message = result.Error })
                : NotFound(new { message = result.Error });
        }

        [HttpPost("final-submission")]
        [ProducesResponseType(typeof(FinalSubmissionStatusDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> SubmitFinal(
            string token, [FromBody] SubmitFinalRequest request, CancellationToken ct)
        {
            var result = await _finalSubmissionService.SubmitAsync(token, request, ct);

            if (result.Succeeded)
                return Ok(result.Data);

            if (result.Error == "This link is invalid or has been revoked.")
                return NotFound(new { message = result.Error });

            if (result.Error == "This approval has already been submitted and is locked.")
                return Conflict(new { message = result.Error });

            return BadRequest(new { message = result.Error });
        }
    }
}
