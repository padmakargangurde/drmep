using Domain.Enums;
using System;
using System.Collections.Generic;

namespace Application.Features.Revisions.DTOs
{
    public class UploadRevisionResult
    {
        public int RevisionId { get; set; }
        public int RevisionNumber { get; set; }
        public string Label { get; set; } = null!;
        public int CategoryCount { get; set; }
        public int TotalItems { get; set; }
        public int AddedCount { get; set; }
        public int ModifiedCount { get; set; }
        public int RemovedCount { get; set; }
        public int UnchangedCount { get; set; }
        public List<string> Warnings { get; set; } = new();
    }

    public class RevisionListItemDto
    {
        public int RevisionId { get; set; }
        public int RevisionNumber { get; set; }
        public string Label { get; set; } = null!;
        public DateTime UploadedAt { get; set; }
        public string? UploadedByName { get; set; }
        public bool IsActive { get; set; }
        public int ItemCount { get; set; }
    }

    public class CategoryDto
    {
        public int CategoryId { get; set; }
        public string Name { get; set; } = null!;
        public List<string> Columns { get; set; } = new();
        public string? SpecReference { get; set; }
        public List<string> Warnings { get; set; } = new();
    }

    public class CommentEntryDto
    {
        public int? UserId { get; set; }
        public string? Email { get; set; }
        public string Comment { get; set; } = null!;
        public string? ReviewedByName { get; set; }
        public string? Role { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class ApprovalItemDto
    {
        public int ItemId { get; set; }
        public int ItemIdentityId { get; set; }
        public string CategoryName { get; set; } = null!;
        public string? Section { get; set; }
        public int ItemNo { get; set; }
        public Dictionary<string, string?> Fields { get; set; } = new();
        public string? SourceStatus { get; set; }
        public string? SpecReference { get; set; }
        public bool LowConfidence { get; set; }
        public ItemChangeType ChangeType { get; set; }
        public ReviewDecision Decision { get; set; } = ReviewDecision.Pending;
        public string? Comment { get; set; }
        public string? ReviewedByName { get; set; }
        public DateTime? RespondedAt { get; set; }

        /// <summary>Full comment thread for this item — multiple reviewers over time, oldest first.</summary>
        public List<CommentEntryDto> Comments { get; set; } = new();
    }

    public class RevisionDetailDto
    {
        public int RevisionId { get; set; }
        public int RevisionNumber { get; set; }
        public string Label { get; set; } = null!;
        public bool IsActive { get; set; }
        public List<CategoryDto> Categories { get; set; } = new();
        public List<ApprovalItemDto> Items { get; set; } = new();
    }

    public class ExportedFileDto
    {
        public byte[] Content { get; set; } = Array.Empty<byte>();
        public string FileName { get; set; } = null!;
        public string ContentType { get; set; } = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
    }
}
