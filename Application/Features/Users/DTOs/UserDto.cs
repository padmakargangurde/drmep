using Domain.Enums;
using System;

namespace Application.Features.Users.DTOs
{
    public class CreateUserRequest
    {
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public UserRole Role { get; set; }
    }

    public class UpdateUserRequest
    {
        public string Name { get; set; } = null!;
        public UserRole Role { get; set; }
        public bool IsActive { get; set; }
    }

    public class UserListItemDto
    {
        public int UserId { get; set; }
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public UserRole Role { get; set; }
        public bool IsActive { get; set; }
    }

    public class UserDetailDto
    {
        public int UserId { get; set; }
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public UserRole Role { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
