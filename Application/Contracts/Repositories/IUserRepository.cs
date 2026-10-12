using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Contracts.Repositories
{
    public interface IUserRepository
    {
        Task<IReadOnlyList<AppUser>> GetAllAsync(CancellationToken ct = default);

        Task<AppUser?> GetByIdAsync(int userId, CancellationToken ct = default);

        Task<AppUser?> GetByEmailAsync(string email, CancellationToken ct = default);

        Task<bool> EmailExistsAsync(string email, CancellationToken ct = default);

        Task AddAsync(AppUser user, CancellationToken ct = default);

        Task UpdateAsync(AppUser user, CancellationToken ct = default);

        /// <summary>Soft delete - sets IsDeleted, never removes the row.</summary>
        Task<bool> SoftDeleteAsync(int userId, CancellationToken ct = default);
    }
}
