using Domain.Entities.Base;

namespace Domain.Entities
{
    /// <summary>One worksheet from the uploaded workbook (e.g. "Plumbing Material").</summary>
    public class ApprovalCategory : BaseEntity<int>
    {
        public int ProjectRevisionId { get; set; }
        public ProjectRevision ProjectRevision { get; set; } = null!;

        public string Name { get; set; } = null!;

        /// <summary>Column headers in sheet order. Stored as JSON — dynamic per sheet.</summary>
        public List<string> Columns { get; set; } = new();

        public string? SpecReference { get; set; }

        /// <summary>Parser warnings (sparse rows, unrecognized headers, etc).</summary>
        public List<string> Warnings { get; set; } = new();
    }
}
