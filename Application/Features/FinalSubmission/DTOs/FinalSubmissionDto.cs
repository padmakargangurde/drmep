using System;

namespace Application.Features.FinalSubmission.DTOs
{
    public class SubmitFinalRequest
    {
        public string SubmittedByName { get; set; } = null!;
        public string? SubmittedByCompany { get; set; }
    }

    public class FinalSubmissionStatusDto
    {
        public bool IsLocked { get; set; }
        public string? SubmittedByName { get; set; }
        public string? SubmittedByCompany { get; set; }
        public DateTime? SubmittedAt { get; set; }
        public int? ApprovedCount { get; set; }
        public int? RejectedCount { get; set; }
        public int? ClarifyCount { get; set; }
        public int? NaCount { get; set; }
        public int? CommentedCount { get; set; }
        public DateTime? ReopenedAt { get; set; }
    }
}
