using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Contracts.Comments
{
    /// <summary>One reviewer's comment against a specific item, as stored in MEP-APIs-V2's MongoDB Comments collection.</summary>
    public class CommentEntry
    {
        /// <summary>Optional — AppUser.Id of the commenter, when known (null for the Client flow).</summary>
        public int? UserId { get; set; }

        /// <summary>Optional — the commenter's email, when known.</summary>
        public string? Email { get; set; }

        public string Comment { get; set; } = null!;
        public string? ReviewedByName { get; set; }

        /// <summary>Who left this comment — "Client", or an internal UserRole (ProjectManager/BimCoordinator/Admin).</summary>
        public string? Role { get; set; }

        public DateTime CreatedAt { get; set; }
    }

    /// <summary>
    /// Talks to MEP-APIs-V2 for an item's comment thread — unlike Decision (a single current status kept
    /// in SQL), comments accumulate over time from multiple reviewers and live in MongoDB, one document
    /// per item holding the whole thread.
    /// </summary>
    public interface ICommentsClient
    {
        /// <summary>Appends a new comment to the item's thread — never overwrites, always adds a new entry.</summary>
        Task AddCommentAsync(
            int itemIdentityId, string comment, string? reviewedByName, string? role,
            int? userId = null, string? email = null, CancellationToken ct = default);

        /// <summary>Fetches the full comment thread for a batch of item identities, oldest first, keyed by item identity id.</summary>
        Task<Dictionary<int, List<CommentEntry>>> GetCommentsBatchAsync(List<int> itemIdentityIds, CancellationToken ct = default);
    }
}
