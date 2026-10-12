using Domain.Entities.Base;

namespace Domain.Entities
{
    /// <summary>
    /// One uploaded workbook for a project. Re-uploading creates a new revision
    /// rather than overwriting the last one, so prior decisions and the diff
    /// history are never lost.
    /// </summary>
    public class ProjectRevision : BaseEntity<int>
    {
        public int ProjectId { get; set; }
        public Project Project { get; set; } = null!;

        /// <summary>Sequential per project: 1, 2, 3 ...</summary>
        public int RevisionNumber { get; set; }

        /// <summary>e.g. "R01 — Original".</summary>
        public string Label { get; set; } = null!;

        public string SourceFileName { get; set; } = null!;

        public DateTime UploadedAt { get; set; }

        public int? UploadedByUserId { get; set; }
        public AppUser? UploadedBy { get; set; }

        /// <summary>Only one revision per project has this set — the one clients review.</summary>
        public bool IsActive { get; set; }

        public List<ApprovalCategory> Categories { get; set; } = new();
        public List<ApprovalItem> Items { get; set; } = new();
    }
}
