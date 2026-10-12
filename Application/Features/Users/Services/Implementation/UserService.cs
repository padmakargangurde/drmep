using Application.Common.Results;
using Application.Contracts.UnitOfWork;
using Application.Features.Users.DTOs;
using Application.Features.Users.Services;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUOW _uow;

        public UserService(IUOW uow) => _uow = uow;

        public async Task<IReadOnlyList<UserListItemDto>> GetAllAsync(CancellationToken ct = default)
        {
            var users = await _uow.UserRepository.GetAllAsync(ct);

            return users
                .OrderBy(u => u.Name)
                .Select(u => new UserListItemDto
                {
                    UserId = u.Id,
                    Name = u.Name,
                    Email = u.Email,
                    Role = u.Role,
                    IsActive = u.IsActive
                })
                .ToList();
        }

        public async Task<Result<UserDetailDto>> GetByIdAsync(int userId, CancellationToken ct = default)
        {
            var user = await _uow.UserRepository.GetByIdAsync(userId, ct);

            return user is null
                ? Result<UserDetailDto>.Failure("User not found.")
                : Result<UserDetailDto>.Success(ToDetail(user));
        }

        public async Task<Result<UserDetailDto>> CreateAsync(
            CreateUserRequest request, CancellationToken ct = default)
        {
            var email = request.Email?.Trim();

            if (string.IsNullOrWhiteSpace(request.Name))
                return Result<UserDetailDto>.Failure("Name is required.");

            if (string.IsNullOrWhiteSpace(email))
                return Result<UserDetailDto>.Failure("Email is required.");

            // Checked here as well as by the unique index. The index is the real
            // guarantee; this check exists to return a readable message instead
            // of a constraint violation.
            if (await _uow.UserRepository.EmailExistsAsync(email, ct))
                return Result<UserDetailDto>.Failure($"Email '{email}' is already in use.");

            var user = new AppUser
            {
                Name = request.Name.Trim(),
                Email = email,
                Role = request.Role,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            await _uow.UserRepository.AddAsync(user, ct);
            await _uow.SaveChangesAsync(ct);

            return Result<UserDetailDto>.Success(ToDetail(user));
        }

        public async Task<Result<UserDetailDto>> UpdateAsync(
            int userId, UpdateUserRequest request, CancellationToken ct = default)
        {
            var user = await _uow.UserRepository.GetByIdAsync(userId, ct);
            if (user is null)
                return Result<UserDetailDto>.Failure("User not found.");

            if (string.IsNullOrWhiteSpace(request.Name))
                return Result<UserDetailDto>.Failure("Name is required.");

            // Email is intentionally not updatable here — changing it needs its
            // own uniqueness check against the same rule CreateAsync enforces.
            user.Name = request.Name.Trim();
            user.Role = request.Role;
            user.IsActive = request.IsActive;
            user.UpdatedAt = DateTime.UtcNow;

            await _uow.UserRepository.UpdateAsync(user, ct);
            await _uow.SaveChangesAsync(ct);

            return Result<UserDetailDto>.Success(ToDetail(user));
        }

        public async Task<Result<bool>> DeleteAsync(int userId, CancellationToken ct = default)
        {
            var deleted = await _uow.UserRepository.SoftDeleteAsync(userId, ct);
            if (!deleted)
                return Result<bool>.Failure("User not found.");

            await _uow.SaveChangesAsync(ct);
            return Result<bool>.Success(true);
        }

        private static UserDetailDto ToDetail(AppUser u) => new()
        {
            UserId = u.Id,
            Name = u.Name,
            Email = u.Email,
            Role = u.Role,
            IsActive = u.IsActive,
            CreatedAt = u.CreatedAt,
            UpdatedAt = u.UpdatedAt
        };
    }
}
