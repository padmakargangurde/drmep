using Domain.Entities.Base;

namespace Domain.Entities
{
    public class Project : BaseEntity<int>
    {
        /// <summary>Unique among non-deleted projects, e.g. "PTC-PH2".</summary>
        public string ProjectNumber { get; set; } = null!;

        /// <summary>e.g. "Providence Town Center Phase 2".</summary>
        public string Name { get; set; } = null!;

        public string ClientName { get; set; } = null!;
        public string? ClientCompany { get; set; }
        public string? Location { get; set; }

        public int? ProjectManagerUserId { get; set; }
        public AppUser? ProjectManager { get; set; }

        public int? BimCoordinatorUserId { get; set; }
        public AppUser? BimCoordinator { get; set; }

        public DateOnly? DueDate { get; set; }
        public string? Description { get; set; }

        /// <summary>
        /// False until the first workbook is imported. The dashboard shows
        /// these as "Draft - no matrix uploaded".
        /// </summary>
        public bool IsPublished { get; set; }

        /// <summary>
        /// Sheets and columns the admin chose to exclude at import, reused as
        /// defaults for later revisions. Stored as one JSON column.
        /// </summary>
        public ImportSettings? ImportSettings { get; set; }

        // multi-tenant hooks for the host platform
        public string? SiteContext { get; set; }
        public Guid? WorkspaceId { get; set; }

        public int? CreatedByUserId { get; set; }
        public AppUser? CreatedBy { get; set; }

        // Id, CreatedAt, UpdatedAt and IsDeleted come from BaseEntity<int>.
    }

    /// <summary>
    /// Persisted as a single JSON column on Project, not a table: it is read
    /// and written whole, and nothing ever queries an individual entry.
    /// </summary>
    public class ImportSettings
    {
        public List<string> IgnoredSheets { get; set; } = new();
        public List<string> IgnoredColumns { get; set; } = new();
    }
}