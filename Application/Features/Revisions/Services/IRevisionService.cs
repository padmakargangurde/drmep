using Application.Common.Results;
using Application.Features.Revisions.DTOs;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Revisions.Services
{
    /// <summary>
    /// Revision workflow: upload a workbook, diff it against the active revision, list/fetch
    /// revisions, export the active one. A row's Excel Fields live in MongoDB (MEP-APIs-V2);
    /// everything else (Project, ItemIdentity, decisions, audit log) lives in SQL Server.
    /// </summary>
    public interface IRevisionService
    {
        Task<Result<UploadRevisionResult>> UploadAsync(
            int projectId, Stream fileStream, string fileName, int? uploadedByUserId, CancellationToken ct = default);

        Task<Result<IReadOnlyList<RevisionListItemDto>>> GetAllAsync(int projectId, CancellationToken ct = default);

        Task<Result<RevisionDetailDto>> GetActiveAsync(int projectId, CancellationToken ct = default);

        Task<Result<RevisionDetailDto>> GetByIdAsync(int projectId, int revisionId, CancellationToken ct = default);

        Task<Result<ExportedFileDto>> ExportActiveAsync(int projectId, CancellationToken ct = default);
    }
}
