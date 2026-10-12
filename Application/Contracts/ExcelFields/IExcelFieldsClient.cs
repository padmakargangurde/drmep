using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Contracts.ExcelFields
{
    /// <summary>One Excel matrix row's cell data, as stored in MEP-APIs-V2's MongoDB collection.</summary>
    public class ExcelFieldsRow
    {
        public string? Id { get; set; }
        public int ProjectId { get; set; }
        public string? ProjectNumber { get; set; }
        public string? RevisionLabel { get; set; }
        public string CategoryName { get; set; } = null!;
        public string? Section { get; set; }
        public int ItemNo { get; set; }
        public Dictionary<string, string?> Fields { get; set; } = new();
    }

    /// <summary>
    /// Talks to MEP-APIs-V2 for the one piece of a revision's data that lives outside SQL Server:
    /// the Excel sheet's per-row Fields. Everything else about a revision stays in SQL as before.
    /// </summary>
    public interface IExcelFieldsClient
    {
        /// <summary>Inserts new Fields documents and returns their generated ids, in the same order as <paramref name="rows"/>.</summary>
        Task<List<string>> SaveBatchAsync(List<ExcelFieldsRow> rows, CancellationToken ct = default);

        /// <summary>Fetches Fields for a batch of document ids, keyed by id.</summary>
        Task<Dictionary<string, Dictionary<string, string?>>> GetBatchAsync(List<string> documentIds, CancellationToken ct = default);
    }
}
