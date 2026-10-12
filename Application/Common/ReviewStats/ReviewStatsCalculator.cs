using Application.Features.ClientReview.DTOs;
using Application.Features.Revisions.DTOs;
using Domain.Enums;
using System.Collections.Generic;

namespace Application.Common.ReviewStats
{
    /// <summary>Shared by client review (live progress) and final submission (snapshot at sign-off).</summary>
    public static class ReviewStatsCalculator
    {
        public static ReviewStatsDto Compute(List<ApprovalItemDto> items)
        {
            var stats = new ReviewStatsDto { Total = items.Count };

            foreach (var item in items)
            {
                switch (item.Decision)
                {
                    case ReviewDecision.Pending: stats.Pending++; break;
                    case ReviewDecision.Approved: stats.Approved++; break;
                    case ReviewDecision.Rejected: stats.Rejected++; break;
                    case ReviewDecision.Commented: stats.Commented++; break;
                    case ReviewDecision.Clarify: stats.Clarify++; break;
                    case ReviewDecision.NA: stats.Na++; break;
                }
            }

            return stats;
        }
    }
}
