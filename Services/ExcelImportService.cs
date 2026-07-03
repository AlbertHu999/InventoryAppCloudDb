// Services/ExcelImportService.cs
using ClosedXML.Excel;
using System.Data;

namespace InventoryAppCloudDb.Services;

public static class ExcelImportService
{
    // ── 開啟檔案，回傳所有 Sheet 名稱 ──────────────────
    public static List<string> GetSheetNames(string filePath)
    {
        using var workbook = new XLWorkbook(filePath);
        return workbook.Worksheets.Select(ws => ws.Name).ToList();
    }

    // ── 讀取單一 Sheet，轉成 DataTable（全部欄位用 string）──
    public static DataTable ReadSheetToDataTable(string filePath, string sheetName)
    {
        using var workbook = new XLWorkbook(filePath);
        var sheet = workbook.Worksheet(sheetName);

        var dt = new DataTable(sheetName);

        var usedRange = sheet.RangeUsed();
        if (usedRange == null) return dt;   // 空白 Sheet

        var headerRow = usedRange.FirstRow();
        int colCount = headerRow.CellCount();

        // 建立欄位：全部用 string，方便畫面編輯，驗證階段再轉型
        for (int c = 1; c <= colCount; c++)
        {
            var headerText = headerRow.Cell(c).GetString().Trim();
            if (string.IsNullOrEmpty(headerText))
                headerText = $"欄位{c}";

            // 避免重複欄位名稱造成 DataTable 例外
            var finalName = headerText;
            int dup = 1;
            while (dt.Columns.Contains(finalName))
                finalName = $"{headerText}_{dup++}";

            dt.Columns.Add(finalName, typeof(string));
        }

        // 讀取資料列（跳過標題列）
        foreach (var row in usedRange.RowsUsed().Skip(1))
        {
            var dr = dt.NewRow();
            for (int c = 1; c <= colCount; c++)
            {
                dr[c - 1] = row.Cell(c).GetString();
            }
            dt.Rows.Add(dr);
        }

        return dt;
    }
}