using Domain.Entities.Base;
using Domain.Enums;

namespace Domain.Entities
{
    /// <summary>
    /// Internal staff directory entry (Admin, PM, BIM Coordinator). No login yet —
    /// this exists so projects can assign a real person instead of a free-text
    /// name, and so audit entries can attribute an action to a user.
    /// </summary>
    public class AppUser : BaseEntity<int>
    {
        public string Name { get; set; } = null!;

        public string Email { get; set; } = null!;

        public UserRole Role { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
