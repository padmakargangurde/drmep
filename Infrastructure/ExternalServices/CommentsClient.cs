using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Application.Contracts.Comments;

namespace Infrastructure.ExternalServices
{
    /// <summary>HttpClient-based implementation of <see cref="ICommentsClient"/> — calls MEP-APIs-V2's Comments API.</summary>
    public class CommentsClient : ICommentsClient
    {
        private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

        private readonly HttpClient _http;

        public CommentsClient(HttpClient http) => _http = http;

        public async Task AddCommentAsync(
            int itemIdentityId, string comment, string? reviewedByName, string? role,
            int? userId = null, string? email = null, CancellationToken ct = default)
        {
            var entry = new CommentEntry
            {
                UserId = userId,
                Email = email,
                Comment = comment,
                ReviewedByName = reviewedByName,
                Role = role
            };

            var response = await _http.PostAsJsonAsync($"api/Comments/{itemIdentityId}/append", entry, JsonOptions, ct);
            response.EnsureSuccessStatusCode();
        }

        public async Task<Dictionary<int, List<CommentEntry>>> GetCommentsBatchAsync(List<int> itemIdentityIds, CancellationToken ct = default)
        {
            if (itemIdentityIds.Count == 0) return new Dictionary<int, List<CommentEntry>>();

            var response = await _http.PostAsJsonAsync("api/Comments/batch-get", itemIdentityIds, JsonOptions, ct);
            response.EnsureSuccessStatusCode();

            var threads = await response.Content.ReadFromJsonAsync<List<CommentThreadResponse>>(JsonOptions, ct)
                ?? new List<CommentThreadResponse>();

            var result = new Dictionary<int, List<CommentEntry>>();
            foreach (var thread in threads)
                result[thread.ItemIdentityId] = thread.Comments;

            return result;
        }

        private class CommentThreadResponse
        {
            public int ItemIdentityId { get; set; }
            public List<CommentEntry> Comments { get; set; } = new();
        }
    }
}
