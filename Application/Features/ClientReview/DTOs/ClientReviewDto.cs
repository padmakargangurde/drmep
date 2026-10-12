using Application.Features.FinalSubmission.DTOs;
using Application.Features.Revisions.DTOs;
using Domain.Enums;
using System;
using System.Collections.Generic;

namespace Application.Features.ClientReview.DTOs
{
    public class ReviewStatsDto
    {
        public int Total { get; set; }
        public int Pending { get; set; }
        public int Approved { get; set; }
        public int Rejected { get; set; }
        public int Commented { get; set; }
        public int Clarify { get; set; }
        public int Na { get; set; }
    }

    public class ClientReviewDto
    {
        public string ProjectNumber { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string ClientName { get; set; } = null!;
        public string RevisionLabel { get; set; } = null!;
        public ReviewStatsDto ReviewStats { get; set; } = new();
        public FinalSubmissionStatusDto FinalStatus { get; set; } = new();
        public List<CategoryDto> Categories { get; set; } = new();
        public List<ApprovalItemDto> Items { get; set; } = new();
    }

    public class SubmitResponseRequest
    {
        public ReviewDecision Decision { get; set; }
        public string? Comment { get; set; }
        public string? ReviewedByName { get; set; }
        public int? UserId { get; set; }
        public string? Email { get; set; }
    }

    public class IdentifyClientRequest
    {
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
    }

    public class IdentifyClientResult
    {
        public int UserId { get; set; }
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
    }

    public class ItemResponseDto
    {
        public int ItemIdentityId { get; set; }
        public ReviewDecision Decision { get; set; }
        public string? Comment { get; set; }
        public string? ReviewedByName { get; set; }
        public DateTime? RespondedAt { get; set; }
    }
}
