using Domain.Entities.Base;

namespace Domain.Entities
{
    /// <summary>One line in a project's activity trail — admin actions and client responses alike.</summary>
    public class AuditLogEntry : BaseEntity<int>
    {
        public int ProjectId { get; set; }
        public Project Project { get; set; } = null!;

        public DateTime Timestamp { get; set; }

        /// <summary>Null for client actions — clients have no account.</summary>
        public int? ActorUserId { get; set; }
        public AppUser? Actor { get; set; }

        /// <summary>Always set: the admin's name, the client's typed name, or "System".</summary>
        public string ActorDisplayName { get; set; } = null!;

        public string Action { get; set; } = null!;
        public string? Detail { get; set; }
    }
}
