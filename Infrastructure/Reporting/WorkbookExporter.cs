using Application.Contracts.Exporting;
using Application.Features.Revisions.DTOs;
using ClosedXML.Excel;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Infrastructure.Reporting
{
    public class WorkbookExporter : IWorkbookExporter
    {
        public byte[] ExportApprovalMatrix(ExportWorkbookRequest request)
        {
            using var workbook = new XLWorkbook();

            AddSummarySheet(workbook, request);

            foreach (var group in request.Items.GroupBy(i => i.CategoryName))
            {
                AddCategorySheet(workbook, request, group.Key, group.ToList());
            }

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        private static void AddSummarySheet(XLWorkbook workbook, ExportWorkbookRequest request)
        {
            var sheet = workbook.Worksheets.Add("Summary");

            int approved = request.Items.Count(i => i.Decision == ReviewDecision.Approved);
            int commented = request.Items.Count(i => i.Decision == ReviewDecision.Commented);
            int rejected = request.Items.Count(i => i.Decision == ReviewDecision.Rejected);
            int clarify = request.Items.Count(i => i.Decision == ReviewDecision.Clarify);
            int na = request.Items.Count(i => i.Decision == ReviewDecision.NA);
            int pending = request.Items.Count(i => i.Decision == ReviewDecision.Pending);

            var rows = new (string Label, string Value)[]
            {
                ("MEP MATERIAL & INSULATION APPROVAL", ""),
                ("Project", request.ProjectName),
                ("Project Number", request.ProjectNumber),
                ("Client", request.ClientName),
                ("Revision", request.RevisionLabel),
                ("Export Date", DateTime.UtcNow.ToString("yyyy-MM-dd")),
                ("", ""),
                ("Total Items", request.Items.Count.ToString()),
                ("Approved", approved.ToString()),
                ("Approved with Comments", commented.ToString()),
                ("Rejected", rejected.ToString()),
                ("Need Clarification", clarify.ToString()),
                ("Not Applicable", na.ToString()),
                ("Pending", pending.ToString()),
            };

            for (var r = 0; r < rows.Length; r++)
            {
                sheet.Cell(r + 1, 1).Value = rows[r].Label;
                sheet.Cell(r + 1, 2).Value = rows[r].Value;
            }

            sheet.Column(1).Width = 26;
            sheet.Column(2).Width = 40;
        }

        private static void AddCategorySheet(
            XLWorkbook workbook, ExportWorkbookRequest request, string categoryName, IReadOnlyList<ApprovalItemDto> items)
        {
            var category = request.Categories.FirstOrDefault(c => c.Name == categoryName);
            var columns = category?.Columns ?? new List<string>();
            var header = columns
                .Concat(new[] { "Item ID", "Client Response", "Client Comment", "Reviewed By", "Approval Date", "Revision" })
                .ToList();

            var safeName = new string(categoryName.Where(c => !"\\/?*[]:".Contains(c)).ToArray());
            if (safeName.Length > 31) safeName = safeName[..31];
            var sheet = workbook.Worksheets.Add(string.IsNullOrWhiteSpace(safeName) ? "Sheet" : safeName);

            for (var c = 0; c < header.Count; c++)
                sheet.Cell(1, c + 1).Value = header[c];

            var prefix = categoryName.Length >= 2 ? categoryName[..2].ToUpperInvariant() : categoryName.ToUpperInvariant();
            var rowIndex = 2;
            var itemNumber = 1;
            foreach (var item in items)
            {
                var col = 1;
                foreach (var column in columns)
                {
                    sheet.Cell(rowIndex, col).Value = item.Fields.TryGetValue(column, out var value) ? value ?? "" : "";
                    col++;
                }

                sheet.Cell(rowIndex, col++).Value = $"{prefix}-{itemNumber:D3}";
                sheet.Cell(rowIndex, col++).Value = item.Decision.ToString();
                sheet.Cell(rowIndex, col++).Value = item.Comment ?? "";
                sheet.Cell(rowIndex, col++).Value = item.ReviewedByName ?? "";
                sheet.Cell(rowIndex, col++).Value = item.RespondedAt?.ToString("yyyy-MM-dd") ?? "";
                sheet.Cell(rowIndex, col).Value = request.RevisionLabel;

                rowIndex++;
                itemNumber++;
            }

            for (var c = 0; c < header.Count; c++)
                sheet.Column(c + 1).Width = Math.Max(14, Math.Min(38, header[c].Length + 6));

            var used = sheet.RangeUsed();
            if (used is not null) used.SetAutoFilter();
        }
    }
}
