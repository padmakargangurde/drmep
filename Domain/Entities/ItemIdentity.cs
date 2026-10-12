using Domain.Entities.Base;

namespace Domain.Entities
{
    /// <summary>
    /// The stable identity of one matrix row, independent of revision. A new
    /// upload that repeats the same category+section+field values resolves to
    /// the same ItemIdentity, so its <see cref="ItemResponse"/> carries forward
    /// automatically; a changed row gets a new key and starts back at Pending.
    /// </summary>
    public class ItemIdentity : BaseEntity<int>
    {
        public int ProjectId { get; set; }
        public Project Project { get; set; } = null!;

        /// <summary>Hash of category + section + field values.</summary>
        public string IdentityKey { get; set; } = null!;

        public string CategoryName { get; set; } = null!;
        public string? Section { get; set; }

        public ItemResponse? Response { get; set; }
        public List<ApprovalItem> Snapshots { get; set; } = new();
    }
}
