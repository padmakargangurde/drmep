using Domain.Entities.Base;
using Domain.Enums;

namespace Domain.Entities
{
    /// <summary>
    /// The client's current decision for one <see cref="ItemIdentity"/>. Overwritten
    /// in place on every response, so it always reflects the latest word on that item.
    /// </summary>
    public class ItemResponse : BaseEntity<int>
    {
        public int ItemIdentityId { get; set; }
        public ItemIdentity ItemIdentity { get; set; } = null!;

        public ReviewDecision Decision { get; set; } = ReviewDecision.Pending;

        public string? Comment { get; set; }

        /// <summary>Client types a name — there is no reviewer account.</summary>
        public string? ReviewedByName { get; set; }

        public DateTime? RespondedAt { get; set; }
    }
}
