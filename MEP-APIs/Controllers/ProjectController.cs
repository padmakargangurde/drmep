using Application.Features.Projects.DTOs;
using Application.Features.Projects.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MEP_APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectController : ControllerBase
    {

        private readonly IProjectService _projectService;

        public ProjectController(IProjectService projectService)
            => _projectService = projectService;

        /// <summary>Dashboard list.</summary>
        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyList<ProjectListItemDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll(CancellationToken ct)
            => Ok(await _projectService.GetAllAsync(ct));

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(ProjectDetailDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id, CancellationToken ct)
        {
            var result = await _projectService.GetByIdAsync(id, ct);

            return result.Succeeded
                ? Ok(result.Data)
                : NotFound(new { message = result.Error });
        }

        [HttpPost]
        [ProducesResponseType(typeof(ProjectDetailDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create(
            [FromBody] CreateProjectRequest request, CancellationToken ct)
        {
            var result = await _projectService.CreateAsync(request, ct);

            if (!result.Succeeded)
                return BadRequest(new { message = result.Error });

            return CreatedAtAction(
                nameof(GetById),
                new { id = result.Data!.ProjectId },
                result.Data);
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(ProjectDetailDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(
            int id, [FromBody] UpdateProjectRequest request, CancellationToken ct)
        {
            var result = await _projectService.UpdateAsync(id, request, ct);

            if (result.Succeeded)
                return Ok(result.Data);

            // "not found" and "invalid input" are different statuses; the
            // service distinguishes them only by message, so match on it.
            return result.Error == "Project not found."
                ? NotFound(new { message = result.Error })
                : BadRequest(new { message = result.Error });
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var result = await _projectService.DeleteAsync(id, ct);

            return result.Succeeded
                ? NoContent()
                : NotFound(new { message = result.Error });
        }

    }
}
