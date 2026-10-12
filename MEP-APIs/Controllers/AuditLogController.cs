using Application.Features.AuditLog.Services;
using Microsoft.AspNetCore.Mvc;

namespace MEP_APIs.Controllers
{
    [Route("api/Project/{projectId:int}/audit-log")]
    [ApiController]
    public class AuditLogController : ControllerBase
    {
        private readonly IAuditLogService _auditLogService;

        public AuditLogController(IAuditLogService auditLogService)
            => _auditLogService = auditLogService;

        [HttpGet]
        public async Task<IActionResult> GetForProject(int projectId, CancellationToken ct)
        {
            var result = await _auditLogService.GetForProjectAsync(projectId, ct);

            return result.Succeeded
                ? Ok(result.Data)
                : NotFound(new { message = result.Error });
        }
    }
}
