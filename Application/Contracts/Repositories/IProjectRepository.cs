using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Contracts.Repositories
{
    /// <summary>
    /// Declared here, implemented in Infrastructure. This is the seam that
    /// keeps EF Core out of the handlers: Application depends on this
    /// interface, Infrastructure depends on Application, and nothing points
    /// the other way.
    /// </summary>
    public interface IProjectRepository
    {
        Task<IReadOnlyList<Project>> GetAllAsync(CancellationToken ct = default);

        Task<Project?> GetByIdAsync(int projectId, CancellationToken ct = default);

        Task<bool> ProjectNumberExistsAsync(string projectNumber, CancellationToken ct = default);

        Task AddAsync(Project project, CancellationToken ct = default);

        Task UpdateAsync(Project project, CancellationToken ct = default);

        /// <summary>Soft delete - sets IsDeleted, never removes the row.</summary>
        Task<bool> SoftDeleteAsync(int projectId, CancellationToken ct = default);
    }
}
