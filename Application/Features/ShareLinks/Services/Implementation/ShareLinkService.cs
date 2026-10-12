using Application.Common.Results;
using Application.Contracts.UnitOfWork;
using Application.Features.ShareLinks.DTOs;
using Application.Features.ShareLinks.Services;
using Domain.Entities;
using System;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Services
{
    public class ShareLinkService : IShareLinkService
    {
        private readonly IUOW _uow;

        public ShareLinkService(IUOW uow) => _uow = uow;

        public async Task<Result<ShareLinkDto>> GetActiveAsync(int projectId, CancellationToken ct = default)
        {
            var link = await _uow.ShareLinkRepository.GetActiveByProjectAsync(projectId, ct);

            return link is null
                ? Result<ShareLinkDto>.Failure("This project has no active share link.")
                : Result<ShareLinkDto>.Success(ToDto(link));
        }

        public async Task<Result<ShareLinkDto>> CreateAsync(int projectId, int? createdByUserId, CancellationToken ct = default)
        {
            var project = await _uow.ProjectRepository.GetByIdAsync(projectId, ct);
            if (project is null)
                return Result<ShareLinkDto>.Failure("Project not found.");

            var existing = await _uow.ShareLinkRepository.GetActiveByProjectAsync(projectId, ct);
            if (existing != null)
            {
                existing.IsRevoked = true;
                existing.RevokedAt = DateTime.UtcNow;
            }

            var link = new ShareLink
            {
                ProjectId = projectId,
                Token = GenerateToken(),
                CreatedByUserId = createdByUserId,
                IsRevoked = false,
                CreatedAt = DateTime.UtcNow
            };

            await _uow.ShareLinkRepository.AddAsync(link, ct);
            await _uow.SaveChangesAsync(ct);

            return Result<ShareLinkDto>.Success(ToDto(link));
        }

        public async Task<Result<bool>> RevokeAsync(int projectId, CancellationToken ct = default)
        {
            var link = await _uow.ShareLinkRepository.GetActiveByProjectAsync(projectId, ct);
            if (link is null)
                return Result<bool>.Failure("This project has no active share link.");

            link.IsRevoked = true;
            link.RevokedAt = DateTime.UtcNow;
            await _uow.SaveChangesAsync(ct);

            return Result<bool>.Success(true);
        }

        private static string GenerateToken()
        {
            var bytes = RandomNumberGenerator.GetBytes(24);
            return Convert.ToBase64String(bytes)
                .TrimEnd('=')
                .Replace('+', '-')
                .Replace('/', '_');
        }

        private static ShareLinkDto ToDto(ShareLink link) => new()
        {
            Token = link.Token,
            CreatedAt = link.CreatedAt,
            IsRevoked = link.IsRevoked
        };
    }
}
