using Domain.Entities.Base;

namespace Domain.Entities
{
    /// <summary>
    /// A client's signed-off approval of a revision. Kept as a log rather than
    /// mutated in place: an admin "reopen" sets <see cref="ReopenedAt"/> instead
    /// of deleting the row, so submission history survives. A project is
    /// currently locked when its latest submission has no ReopenedAt.
    /// </summary>
    public class FinalSubmission : BaseEntity<int>
    {
        public int ProjectId { get; set; }
        public Project Project { get; set; } = null!;

        public int ProjectRevisionId { get; set; }
        public ProjectRevision ProjectRevision { get; set; } = null!;

        public string SubmittedByName { get; set; } = null!;
        public string? SubmittedByCompany { get; set; }
        public DateTime SubmittedAt { get; set; }

        public int ApprovedCount { get; set; }
        public int RejectedCount { get; set; }
        public int ClarifyCount { get; set; }
        public int NaCount { get; set; }
        public int CommentedCount { get; set; }

        public DateTime? ReopenedAt { get; set; }
        public int? ReopenedByUserId { get; set; }
        public AppUser? ReopenedBy { get; set; }
    }
}
