using Application.Features.FinalSubmission.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MEP_APIs.Controllers
{
    [Route("api/Project/{projectId:int}/final-submission")]
    [ApiController]
    public class FinalSubmissionController : ControllerBase
    {
        private readonly IFinalSubmissionService _finalSubmissionService;

        public FinalSubmissionController(IFinalSubmissionService finalSubmissionService)
            => _finalSubmissionService = finalSubmissionService;

        [HttpGet]
        public async Task<IActionResult> GetStatus(int projectId, CancellationToken ct)
        {
            var result = await _finalSubmissionService.GetStatusAsync(projectId, ct);

            return result.Succeeded
                ? Ok(result.Data)
                : NotFound(new { message = result.Error });
        }

        [HttpPost("reopen")]
        public async Task<IActionResult> Reopen(int projectId, [FromQuery] int? reopenedByUserId, CancellationToken ct)
        {
            var result = await _finalSubmissionService.ReopenAsync(projectId, reopenedByUserId, ct);

            return result.Succeeded
                ? Ok(result.Data)
                : BadRequest(new { message = result.Error });
        }
    }
}
