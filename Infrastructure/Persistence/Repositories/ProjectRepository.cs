using Application.Contracts.Repositories;
using Domain.Entities;
using Infrastructure.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Repositories
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ProjectRepository> _logger;

        public ProjectRepository(AppDbContext context, ILogger<ProjectRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task AddAsync(Project project, CancellationToken ct = default)
        {
            await _context.Projects.AddAsync(project, ct);
        }

        public async Task<IReadOnlyList<Project>> GetAllAsync(CancellationToken ct = default)
        {
            return await _context.Projects
                .Include(p => p.ProjectManager)
                .Include(p => p.BimCoordinator)
                .AsNoTracking()
                .ToListAsync(ct);
        }

        public async Task<Project?> GetByIdAsync(int projectId, CancellationToken ct = default)
        {
            return await _context.Projects
                .Include(p => p.ProjectManager)
                .Include(p => p.BimCoordinator)
                .FirstOrDefaultAsync(p => p.Id == projectId, ct);
        }

        public async Task<bool> ProjectNumberExistsAsync(string projectNumber, CancellationToken ct = default)
        {
            return await _context.Projects
                .AnyAsync(p => p.ProjectNumber == projectNumber, ct);
        }

        public Task<bool> SoftDeleteAsync(int projectId, CancellationToken ct = default)
        {
            return SoftDeleteInternalAsync(projectId, ct);
        }

        private async Task<bool> SoftDeleteInternalAsync(int projectId, CancellationToken ct)
        {
            var project = await _context.Projects
                .FirstOrDefaultAsync(p => p.Id == projectId, ct);

            if (project is null)
                return false;

            project.IsDeleted = true;
            project.UpdatedAt = DateTime.UtcNow;
            return true;
        }

        public Task UpdateAsync(Project project, CancellationToken ct = default)
        {
            _context.Projects.Update(project);
            return Task.CompletedTask;
        }
    }
}
