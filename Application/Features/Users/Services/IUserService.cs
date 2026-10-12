using Application.Common.Results;
using Application.Features.Users.DTOs;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Users.Services
{
    public interface IUserService
    {
        Task<IReadOnlyList<UserListItemDto>> GetAllAsync(CancellationToken ct = default);

        Task<Result<UserDetailDto>> GetByIdAsync(int userId, CancellationToken ct = default);

        Task<Result<UserDetailDto>> CreateAsync(CreateUserRequest request, CancellationToken ct = default);

        Task<Result<UserDetailDto>> UpdateAsync(int userId, UpdateUserRequest request, CancellationToken ct = default);

        Task<Result<bool>> DeleteAsync(int userId, CancellationToken ct = default);
    }
}
