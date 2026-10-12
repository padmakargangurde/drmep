using Application.Common.Results;
using Application.Features.Projects.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Projects.Services
{
    public interface IProjectService
    {
        Task<IReadOnlyList<ProjectListItemDto>> GetAllAsync(CancellationToken ct = default);

        Task<Result<ProjectDetailDto>> GetByIdAsync(int projectId, CancellationToken ct = default);

        Task<Result<ProjectDetailDto>> CreateAsync(CreateProjectRequest request, CancellationToken ct = default);

        Task<Result<ProjectDetailDto>> UpdateAsync(int projectId, UpdateProjectRequest request, CancellationToken ct = default);

        Task<Result<bool>> DeleteAsync(int projectId, CancellationToken ct = default);
    }
}
