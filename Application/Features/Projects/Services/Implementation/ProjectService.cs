using Application.Common.Results;
using Application.Contracts.UnitOfWork;
using Application.Features.Projects.DTOs;
using Application.Features.Projects.Services;
using Domain.Entities;

namespace Application.Services
{
    public class ProjectService : IProjectService
    {
        private readonly IUOW _uow;

        public ProjectService(IUOW uow) => _uow = uow;

        public async Task<IReadOnlyList<ProjectListItemDto>> GetAllAsync(CancellationToken ct = default)
        {
            var projects = await _uow.ProjectRepository.GetAllAsync(ct);

            return projects
                .OrderByDescending(p => p.CreatedAt)
                .Select(p => new ProjectListItemDto
                {
                    ProjectId = p.Id,
                    ProjectNumber = p.ProjectNumber,
                    Name = p.Name,
                    ClientName = p.ClientName,
                    DueDate = p.DueDate,
                    IsPublished = p.IsPublished,
                    CreatedAt = p.CreatedAt
                })
                .ToList();
        }

        public async Task<Result<ProjectDetailDto>> GetByIdAsync(int projectId, CancellationToken ct = default)
        {
            var project = await _uow.ProjectRepository.GetByIdAsync(projectId, ct);

            return project is null
                ? Result<ProjectDetailDto>.Failure("Project not found.")
                : Result<ProjectDetailDto>.Success(ToDetail(project));
        }

        public async Task<Result<ProjectDetailDto>> CreateAsync(
            CreateProjectRequest request, CancellationToken ct = default)
        {
            var projectNumber = request.ProjectNumber?.Trim();

            if (string.IsNullOrWhiteSpace(projectNumber))
                return Result<ProjectDetailDto>.Failure("Project number is required.");

            if (string.IsNullOrWhiteSpace(request.Name))
                return Result<ProjectDetailDto>.Failure("Project name is required.");

            if (string.IsNullOrWhiteSpace(request.ClientName))
                return Result<ProjectDetailDto>.Failure("Client name is required.");

            // Checked here as well as by the unique index. The index is the real
            // guarantee; this check exists to return a readable message instead
            // of a constraint violation.
            if (await _uow.ProjectRepository.ProjectNumberExistsAsync(projectNumber, ct))
                return Result<ProjectDetailDto>.Failure(
                    $"Project number '{projectNumber}' is already in use.");

            var project = new Project
            {
                ProjectNumber = projectNumber,
                Name = request.Name.Trim(),
                ClientName = request.ClientName.Trim(),
                ClientCompany = request.ClientCompany?.Trim(),
                Location = request.Location?.Trim(),
                ProjectManagerUserId = request.ProjectManagerUserId,
                BimCoordinatorUserId = request.BimCoordinatorUserId,
                DueDate = request.DueDate,
                Description = request.Description,

                // No workbook yet, so the dashboard shows this as a draft.
                IsPublished = false,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            await _uow.ProjectRepository.AddAsync(project, ct);
            await _uow.SaveChangesAsync(ct);

            return Result<ProjectDetailDto>.Success(ToDetail(project));
        }

        public async Task<Result<ProjectDetailDto>> UpdateAsync(
            int projectId, UpdateProjectRequest request, CancellationToken ct = default)
        {
            var project = await _uow.ProjectRepository.GetByIdAsync(projectId, ct);
            if (project is null)
                return Result<ProjectDetailDto>.Failure("Project not found.");

            if (string.IsNullOrWhiteSpace(request.Name))
                return Result<ProjectDetailDto>.Failure("Project name is required.");

            // ProjectNumber is intentionally not updatable. It identifies the
            // project in the client's own documents and in exported filenames.
            project.Name = request.Name.Trim();
            project.ClientName = request.ClientName.Trim();
            project.ClientCompany = request.ClientCompany?.Trim();
            project.Location = request.Location?.Trim();
            project.ProjectManagerUserId = request.ProjectManagerUserId;
            project.BimCoordinatorUserId = request.BimCoordinatorUserId;
            project.DueDate = request.DueDate;
            project.Description = request.Description;
            project.UpdatedAt = DateTime.UtcNow;

           await  _uow.ProjectRepository.UpdateAsync(project);
            await _uow.SaveChangesAsync(ct);

            return Result<ProjectDetailDto>.Success(ToDetail(project));
        }

        public async Task<Result<bool>> DeleteAsync(int projectId, CancellationToken ct = default)
        {
            var deleted = await _uow.ProjectRepository.SoftDeleteAsync(projectId, ct);
            if (!deleted)
                return Result<bool>.Failure("Project not found.");

            await _uow.SaveChangesAsync(ct);
            return Result<bool>.Success(true);
        }

        private static ProjectDetailDto ToDetail(Project p) => new()
        {
            ProjectId = p.Id,
            ProjectNumber = p.ProjectNumber,
            Name = p.Name,
            ClientName = p.ClientName,
            ClientCompany = p.ClientCompany,
            Location = p.Location,
            ProjectManagerUserId = p.ProjectManagerUserId,
            ProjectManagerName = p.ProjectManager?.Name,
            BimCoordinatorUserId = p.BimCoordinatorUserId,
            BimCoordinatorName = p.BimCoordinator?.Name,
            DueDate = p.DueDate,
            Description = p.Description,
            IsPublished = p.IsPublished,
            CreatedAt = p.CreatedAt,
            UpdatedAt = p.UpdatedAt
        };
    }
}