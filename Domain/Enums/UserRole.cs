namespace Domain.Enums
{
    public enum UserRole
    {
        Admin,
        ProjectManager,
        BimCoordinator,

        /// <summary>An external reviewer identified via a project's share link — no login, no internal access.</summary>
        Client
    }
}
