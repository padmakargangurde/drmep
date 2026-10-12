using Application.Features.Revisions.DTOs;
using Application.Features.Revisions.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MEP_APIs.Controllers
{
    /// <summary>
    /// Swashbuckle rejects [FromForm] applied directly to an IFormFile parameter —
    /// wrapping the form fields in a model is the documented workaround.
    /// </summary>
    public class UploadRevisionForm
    {
        public IFormFile File { get; set; } = null!;
        public int? UploadedByUserId { get; set; }
    }

    [Route("api/Project/{projectId:int}/revisions")]
    [ApiController]
    public class RevisionController : ControllerBase
    {
        private const long MaxFileSizeBytes = 25 * 1024 * 1024;
        private static readonly string[] AllowedExtensions = { ".xlsx" };

        private readonly IRevisionService _revisionService;

        public RevisionController(IRevisionService revisionService)
            => _revisionService = revisionService;

        [HttpPost]
        [RequestSizeLimit(MaxFileSizeBytes)]
        [ProducesResponseType(typeof(UploadRevisionResult), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Upload(
            int projectId, [FromForm] UploadRevisionForm form, CancellationToken ct)
        {
            var file = form.File;
            if (file is null || file.Length == 0)
                return BadRequest(new { message = "A workbook file is required." });

            var extension = Path.GetExtension(file.FileName);
            if (!AllowedExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase))
                return BadRequest(new { message = "Only .xlsx workbooks are supported." });

            if (file.Length > MaxFileSizeBytes)
                return BadRequest(new { message = "File exceeds the 25 MB limit." });

            await using var stream = file.OpenReadStream();
            var result = await _revisionService.UploadAsync(projectId, stream, file.FileName, form.UploadedByUserId, ct);

            if (!result.Succeeded)
                return result.Error == "Project not found."
                    ? NotFound(new { message = result.Error })
                    : BadRequest(new { message = result.Error });

            return CreatedAtAction(
                nameof(GetById),
                new { projectId, revisionId = result.Data!.RevisionId },
                result.Data);
        }

        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyList<RevisionListItemDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAll(int projectId, CancellationToken ct)
        {
            var result = await _revisionService.GetAllAsync(projectId, ct);

            return result.Succeeded
                ? Ok(result.Data)
                : NotFound(new { message = result.Error });
        }

        [HttpGet("active")]
        [ProducesResponseType(typeof(RevisionDetailDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetActive(int projectId, CancellationToken ct)
        {
            var result = await _revisionService.GetActiveAsync(projectId, ct);
            return result.Succeeded
                ? Ok(result.Data)
                : NotFound(new { message = result.Error });
        }

        [HttpGet("{revisionId:int}")]
        [ProducesResponseType(typeof(RevisionDetailDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int projectId, int revisionId, CancellationToken ct)
        {
            var result = await _revisionService.GetByIdAsync(projectId, revisionId, ct);

            return result.Succeeded
                ? Ok(result.Data)
                : NotFound(new { message = result.Error });
        }

        [HttpGet("export")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Export(int projectId, CancellationToken ct)
        {
            var result = await _revisionService.ExportActiveAsync(projectId, ct);

            if (!result.Succeeded)
                return NotFound(new { message = result.Error });

            return File(result.Data!.Content, result.Data.ContentType, result.Data.FileName);
        }
    }
}
