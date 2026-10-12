using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Application.Common.Hashing
{
    /// <summary>
    /// Content-based identity for a matrix row: same category + section + field
    /// values always produces the same key, so a re-upload that repeats a row
    /// resolves to the same <c>ItemIdentity</c> and its response carries forward.
    /// </summary>
    public static class IdentityKeyHasher
    {
        public static string ComputeKey(string categoryName, string? section, Dictionary<string, string?> fields)
        {
            var normalizedFields = fields
                .OrderBy(kv => kv.Key, StringComparer.Ordinal)
                .Select(kv => $"{kv.Key}={(kv.Value ?? string.Empty).Trim().ToLowerInvariant()}");

            var raw = $"{categoryName}::{section ?? string.Empty}::{string.Join("|", normalizedFields)}";

            var hash = SHA256.HashData(Encoding.UTF8.GetBytes(raw));
            return Convert.ToHexString(hash).ToLowerInvariant();
        }
    }
}
