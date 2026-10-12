using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Projects.DTOs
{

    /// <summary>
    /// What the client sends to create a project. Deliberately not the entity:
    /// ProjectId, CreatedAt, IsPublished and IsDeleted are ours to set, not
    /// theirs to send.
    /// </summary>
    public class CreateProjectRequest
    {
        public string ProjectNumber { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string ClientName { get; set; } = null!;
        public string? ClientCompany { get; set; }
        public string? Location { get; set; }
        public int? ProjectManagerUserId { get; set; }
        public int? BimCoordinatorUserId { get; set; }
        public DateOnly? DueDate { get; set; }
        public string? Description { get; set; }
    }

    public class UpdateProjectRequest
    {
        public string Name { get; set; } = null!;
        public string ClientName { get; set; } = null!;
        public string? ClientCompany { get; set; }
        public string? Location { get; set; }
        public int? ProjectManagerUserId { get; set; }
        public int? BimCoordinatorUserId { get; set; }
        public DateOnly? DueDate { get; set; }
        public string? Description { get; set; }
    }

    /// <summary>Dashboard card. Only what the list actually renders.</summary>
    public class ProjectListItemDto
    {
        public int ProjectId { get; set; }
        public string ProjectNumber { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string ClientName { get; set; } = null!;
        public DateOnly? DueDate { get; set; }

        /// <summary>False until the first workbook is imported — shows as "Draft".</summary>
        public bool IsPublished { get; set; }

        public DateTime CreatedAt { get; set; }
    }

    public class ProjectDetailDto
    {
        public int ProjectId { get; set; }
        public string ProjectNumber { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string ClientName { get; set; } = null!;
        public string? ClientCompany { get; set; }
        public string? Location { get; set; }
        public int? ProjectManagerUserId { get; set; }
        public string? ProjectManagerName { get; set; }
        public int? BimCoordinatorUserId { get; set; }
        public string? BimCoordinatorName { get; set; }
        public DateOnly? DueDate { get; set; }
        public string? Description { get; set; }
        public bool IsPublished { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
