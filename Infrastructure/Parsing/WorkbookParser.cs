using Application.Contracts.Parsing;
using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Infrastructure.Parsing
{
    /// <summary>
    /// Dynamic .xlsx parser — detects header rows, forward-fills merged cells,
    /// detects section-heading rows, never invents or silently drops data. Port
    /// of the prototype's SheetJS-based parser (mep-approval-platform.html) onto
    /// ClosedXML.
    /// </summary>
    public class WorkbookParser : IWorkbookParser
    {
        private static readonly string[] HeaderKeywords =
        {
            "SYSTEM", "TYPE", "LOCATION", "SIZE", "MATERIAL", "FITTING", "STATUS",
            "INSULATION", "THICKNESS", "REMARK", "DUCT", "LINER", "GAUGE", "GUAGE",
            "SEAL", "PIPE", "CLASS", "SPEC", "EQUIPMENT", "SCHEDULE", "MANUFACTURER",
            "MODEL", "FINISH", "RATING"
        };

        private static readonly Regex SixDigitNumber = new(@"\b\d{6}\b", RegexOptions.Compiled);
        private static readonly Regex StatusHeader = new("STATUS", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public IReadOnlyList<ParsedCategory> Parse(Stream xlsxStream)
        {
            using var workbook = new XLWorkbook(xlsxStream);

            return workbook.Worksheets
                .Select(ParseSheet)
                .ToList();
        }

        private static ParsedCategory ParseSheet(IXLWorksheet ws)
        {
            var usedRange = ws.RangeUsed();
            if (usedRange is null)
            {
                return new ParsedCategory
                {
                    Name = ws.Name,
                    Unparsed = true,
                    Warnings = { "Sheet appears empty." }
                };
            }

            var (grid, headingRows) = BuildGrid(ws, usedRange);
            var headerIdx = FindHeaderRow(grid, headingRows);

            if (headerIdx is null)
            {
                return new ParsedCategory
                {
                    Name = ws.Name,
                    Unparsed = true,
                    Warnings = { "No identifiable header row was found — this sheet was skipped. Use the mapping tool to interpret it manually." }
                };
            }

            var headerRow = grid[headerIdx.Value];
            var colMap = new List<(int Idx, string Label)>();
            for (var i = 0; i < headerRow.Length; i++)
            {
                if (!string.IsNullOrEmpty(headerRow[i]))
                    colMap.Add((i, headerRow[i]!.Trim()));
            }

            var statusColLabel = colMap.Select(c => c.Label).FirstOrDefault(l => StatusHeader.IsMatch(l));
            var specReference = FindSpecReference(grid, headerIdx.Value);

            var items = new List<ParsedItem>();
            string? currentSection = null;
            var blankStreak = 0;
            var unparsedRows = 0;
            var n = 1;

            for (var r = headerIdx.Value + 1; r < grid.Count; r++)
            {
                var row = grid[r];
                var mapped = colMap.Select(c => c.Idx < row.Length ? row[c.Idx] : null).ToList();
                var nonBlank = mapped.Where(v => !string.IsNullOrEmpty(v)).ToList();

                if (nonBlank.Count == 0)
                {
                    blankStreak++;
                    if (blankStreak >= 2) break;
                    continue;
                }
                blankStreak = 0;

                if (headingRows.Contains(r))
                {
                    currentSection = nonBlank[0];
                    continue;
                }

                var distinct = nonBlank.Distinct().ToList();
                if (nonBlank.Count >= 2 && distinct.Count == 1)
                {
                    // Merged note / section divider row (identical text repeated across columns).
                    currentSection = distinct[0];
                    continue;
                }

                var fields = new Dictionary<string, string?>();
                for (var i = 0; i < colMap.Count; i++)
                    fields[colMap[i].Label] = mapped[i];

                var status = statusColLabel != null ? fields.GetValueOrDefault(statusColLabel) : null;
                if (statusColLabel != null)
                    fields.Remove(statusColLabel);

                // A row lacking content in at least half its columns is flagged, not
                // dropped — it still becomes an item so nothing is silently lost, but
                // is marked for admin review.
                var lowConfidence = nonBlank.Count < Math.Ceiling(colMap.Count / 2.0);
                if (lowConfidence) unparsedRows++;

                items.Add(new ParsedItem
                {
                    ItemNo = n++,
                    Section = currentSection,
                    Fields = fields,
                    SourceStatus = status,
                    SpecReference = specReference,
                    LowConfidence = lowConfidence
                });
            }

            var warnings = new List<string>();
            if (unparsedRows > 0)
                warnings.Add($"{unparsedRows} row(s) had unusually sparse data and were flagged for manual review.");

            var columns = colMap
                .Select(c => c.Label)
                .Where(l => !StatusHeader.IsMatch(l))
                .ToList();

            return new ParsedCategory
            {
                Name = ws.Name,
                Columns = columns,
                SpecReference = specReference,
                Warnings = warnings,
                Items = items
            };
        }

        private static (List<string?[]> Grid, HashSet<int> HeadingRows) BuildGrid(IXLWorksheet ws, IXLRange usedRange)
        {
            var firstRow = usedRange.RangeAddress.FirstAddress.RowNumber;
            var firstCol = usedRange.RangeAddress.FirstAddress.ColumnNumber;
            var lastRow = usedRange.RangeAddress.LastAddress.RowNumber;
            var lastCol = usedRange.RangeAddress.LastAddress.ColumnNumber;

            var rows = lastRow - firstRow + 1;
            var cols = lastCol - firstCol + 1;

            var grid = new List<string?[]>(rows);
            for (var r = 0; r < rows; r++)
            {
                var rowArr = new string?[cols];
                for (var c = 0; c < cols; c++)
                    rowArr[c] = CellText(ws.Cell(firstRow + r, firstCol + c));
                grid.Add(rowArr);
            }

            var headingRows = new HashSet<int>();
            foreach (var merge in ws.MergedRanges)
            {
                var r0 = merge.RangeAddress.FirstAddress.RowNumber - firstRow;
                var r1 = merge.RangeAddress.LastAddress.RowNumber - firstRow;
                var c0 = merge.RangeAddress.FirstAddress.ColumnNumber - firstCol;
                var c1 = merge.RangeAddress.LastAddress.ColumnNumber - firstCol;

                if (r0 < 0 || c0 < 0 || r1 >= rows || c1 >= cols)
                    continue;

                var topVal = grid[r0][c0];
                for (var r = r0; r <= r1; r++)
                    for (var c = c0; c <= c1; c++)
                        grid[r][c] = topVal;

                var width = c1 - c0 + 1;
                if (r0 == r1 && width >= 4)
                    headingRows.Add(r0);
            }

            return (grid, headingRows);
        }

        private static string? CellText(IXLCell cell)
        {
            if (cell.IsEmpty()) return null;
            var s = cell.GetString().Trim();
            return s.Length > 0 ? s : null;
        }

        private static int? FindHeaderRow(List<string?[]> grid, HashSet<int> headingRows, int limit = 12)
        {
            int? bestIdx = null;
            var bestScore = 0;

            for (var i = 0; i < Math.Min(limit, grid.Count); i++)
            {
                if (headingRows.Contains(i)) continue;

                var row = grid[i];
                var distinctCount = row.Where(c => !string.IsNullOrEmpty(c)).Distinct().Count();
                if (distinctCount < 2) continue;

                var score = row.Count(cell => !string.IsNullOrEmpty(cell)
                    && HeaderKeywords.Any(k => cell!.ToUpperInvariant().Contains(k)));

                if (score > bestScore)
                {
                    bestScore = score;
                    bestIdx = i;
                }
            }

            return bestScore >= 2 ? bestIdx : null;
        }

        private static string? FindSpecReference(List<string?[]> grid, int headerIdx)
        {
            for (var i = 0; i < headerIdx; i++)
            {
                foreach (var cell in grid[i])
                {
                    if (string.IsNullOrEmpty(cell)) continue;
                    if (cell.Contains("SPEC", StringComparison.OrdinalIgnoreCase) || SixDigitNumber.IsMatch(cell))
                        return cell.Trim();
                }
            }

            return null;
        }
    }
}
