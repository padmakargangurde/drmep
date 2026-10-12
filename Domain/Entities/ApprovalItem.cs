using Domain.Entities.Base;
using Domain.Enums;

namespace Domain.Entities
{
    /// <summary>One matrix row as it appeared in a specific revision.</summary>
    public class ApprovalItem : BaseEntity<int>
    {
        public int ProjectRevisionId { get; set; }
        public ProjectRevision ProjectRevision { get; set; } = null!;

        public int ApprovalCategoryId { get; set; }
        public ApprovalCategory ApprovalCategory { get; set; } = null!;

        public int ItemIdentityId { get; set; }
        public ItemIdentity ItemIdentity { get; set; } = null!;

        public int ItemNo { get; set; }
        public string? Section { get; set; }

        /// <summary>Id of the MongoDB document (MEP-APIs-V2) holding this row's Fields.</summary>
        public string? FieldsDocumentId { get; set; }

        /// <summary>Dynamic column values for this row, keyed by column name — lives only in MongoDB, not mapped to SQL.</summary>
        public Dictionary<string, string?> Fields { get; set; } = new();

        public string? SourceStatus { get; set; }
        public string? SpecReference { get; set; }
        public bool LowConfidence { get; set; }

        public ItemChangeType ChangeType { get; set; }

        /// <summary>Previous revision's snapshot this was diffed against, if Modified.</summary>
        public int? PreviousItemId { get; set; }
        public ApprovalItem? PreviousItem { get; set; }
    }
}
