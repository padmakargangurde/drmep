using System.Collections.Generic;

namespace Application.Contracts.Parsing
{
    /// <summary>One worksheet as parsed from an uploaded workbook, before any diffing.</summary>
    public class ParsedCategory
    {
        public string Name { get; set; } = null!;
        public List<string> Columns { get; set; } = new();
        public string? SpecReference { get; set; }
        public List<string> Warnings { get; set; } = new();
        public List<ParsedItem> Items { get; set; } = new();

        /// <summary>No identifiable header row was found — sheet was skipped, not guessed at.</summary>
        public bool Unparsed { get; set; }
    }

    public class ParsedItem
    {
        public int ItemNo { get; set; }
        public string? Section { get; set; }
        public Dictionary<string, string?> Fields { get; set; } = new();
        public string? SourceStatus { get; set; }
        public string? SpecReference { get; set; }
        public bool LowConfidence { get; set; }
    }
}
