using Domain.Entities.Base;

namespace Domain.Entities
{
    /// <summary>The token a client uses to open a project's review without an account.</summary>
    public class ShareLink : BaseEntity<int>
    {
        public int ProjectId { get; set; }
        public Project Project { get; set; } = null!;

        public string Token { get; set; } = null!;

        public int? CreatedByUserId { get; set; }
        public AppUser? CreatedBy { get; set; }

        public bool IsRevoked { get; set; }
        public DateTime? RevokedAt { get; set; }
    }
}
