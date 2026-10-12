using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Application.Contracts.ExcelFields;

namespace Infrastructure.ExternalServices
{
    /// <summary>HttpClient-based implementation of <see cref="IExcelFieldsClient"/> — calls MEP-APIs-V2's ExcelSheetData API.</summary>
    public class ExcelFieldsClient : IExcelFieldsClient
    {
        private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

        private readonly HttpClient _http;

        public ExcelFieldsClient(HttpClient http) => _http = http;

        public async Task<List<string>> SaveBatchAsync(List<ExcelFieldsRow> rows, CancellationToken ct = default)
        {
            if (rows.Count == 0) return new List<string>();

            var response = await _http.PostAsJsonAsync("api/ExcelSheetData/batch", rows, JsonOptions, ct);
            response.EnsureSuccessStatusCode();

            var saved = await response.Content.ReadFromJsonAsync<List<ExcelFieldsRow>>(JsonOptions, ct)
                ?? new List<ExcelFieldsRow>();

            return saved.Select(r => r.Id!).ToList();
        }

        public async Task<Dictionary<string, Dictionary<string, string?>>> GetBatchAsync(List<string> documentIds, CancellationToken ct = default)
        {
            if (documentIds.Count == 0) return new Dictionary<string, Dictionary<string, string?>>();

            var response = await _http.PostAsJsonAsync("api/ExcelSheetData/batch-get", documentIds, JsonOptions, ct);
            response.EnsureSuccessStatusCode();

            var rows = await response.Content.ReadFromJsonAsync<List<ExcelFieldsRow>>(JsonOptions, ct)
                ?? new List<ExcelFieldsRow>();

            return rows.ToDictionary(r => r.Id!, r => r.Fields);
        }
    }
}
