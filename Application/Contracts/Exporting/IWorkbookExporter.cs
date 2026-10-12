using Application.Features.Revisions.DTOs;
using System.Collections.Generic;

namespace Application.Contracts.Exporting
{
    public class ExportWorkbookRequest
    {
        public int? ProjectId {  get; set; }
        public string ProjectNumber { get; set; } = null!;
        public string ProjectName { get; set; } = null!;
        public string ClientName { get; set; } = null!;
        public string RevisionLabel { get; set; } = null!;
        public IReadOnlyList<CategoryDto> Categories { get; set; } = new List<CategoryDto>();
        public IReadOnlyList<ApprovalItemDto> Items { get; set; } = new List<ApprovalItemDto>();
    }

    /// <summary>
    /// Declared here, implemented in Infrastructure with ClosedXML — mirrors the
    /// IWorkbookParser seam so the Excel library stays out of Application.
    /// </summary>
    public interface IWorkbookExporter
    {
        byte[] ExportApprovalMatrix(ExportWorkbookRequest request);
    }
}
