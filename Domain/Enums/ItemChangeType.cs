namespace Domain.Enums
{
    /// <summary>How an item in a revision compares to the previous revision's snapshot.</summary>
    public enum ItemChangeType
    {
        Added,
        Modified,
        Unchanged,
        Removed
    }
}
