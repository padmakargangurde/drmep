using System;

namespace Application.Features.AuditLog.DTOs
{
    public class AuditLogEntryDto
    {
        public DateTime Timestamp { get; set; }
        public string ActorDisplayName { get; set; } = null!;
        public string Action { get; set; } = null!;
        public string? Detail { get; set; }
    }
}
