namespace Domain.Enums
{
    /// <summary>Client's decision on a single approval item.</summary>
    public enum ReviewDecision
    {
        Pending,
        Approved,
        Rejected,
        Commented,
        Clarify,
        NA
    }
}
