using Application.Contracts.Repositories;
using Domain.Entities;
using Infrastructure.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(AppUser user, CancellationToken ct = default)
        {
            await _context.Users.AddAsync(user, ct);
        }

        public async Task<IReadOnlyList<AppUser>> GetAllAsync(CancellationToken ct = default)
        {
            return await _context.Users.AsNoTracking().ToListAsync(ct);
        }

        public async Task<AppUser?> GetByIdAsync(int userId, CancellationToken ct = default)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userId, ct);
        }

        public async Task<AppUser?> GetByEmailAsync(string email, CancellationToken ct = default)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email, ct);
        }

        public async Task<bool> EmailExistsAsync(string email, CancellationToken ct = default)
        {
            return await _context.Users
                .AnyAsync(u => u.Email == email, ct);
        }

        public Task<bool> SoftDeleteAsync(int userId, CancellationToken ct = default)
        {
            return SoftDeleteInternalAsync(userId, ct);
        }

        private async Task<bool> SoftDeleteInternalAsync(int userId, CancellationToken ct)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userId, ct);

            if (user is null)
                return false;

            user.IsDeleted = true;
            user.UpdatedAt = DateTime.UtcNow;
            return true;
        }

        public Task UpdateAsync(AppUser user, CancellationToken ct = default)
        {
            _context.Users.Update(user);
            return Task.CompletedTask;
        }
    }
}
