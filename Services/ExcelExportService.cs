// Services/ExcelExportService.cs
using ClosedXML.Excel;
using InventoryAppCloudDb.DTOs;

namespace InventoryAppCloudDb.Services;

public static class ExcelExportService
{
    // ── 匯出商品庫存表 ─────────────────────────────────
    public static void ExportProducts(List<ProductDto> products, string filePath)
    {
        using var workbook = new XLWorkbook();
        var sheet = workbook.Worksheets.Add("商品庫存表");

        // 標題列
        sheet.Cell(1, 1).Value = "編號";
        sheet.Cell(1, 2).Value = "商品名稱";
        sheet.Cell(1, 3).Value = "售價";
        sheet.Cell(1, 4).Value = "庫存";
        sheet.Cell(1, 5).Value = "分類";
        sheet.Cell(1, 6).Value = "狀態";

        var headerRange = sheet.Range(1, 1, 1, 6);
        headerRange.Style.Font.Bold = true;
        headerRange.Style.Fill.BackgroundColor = XLColor.LightBlue;
        headerRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

        int row = 2;
        foreach (var p in products)
        {
            sheet.Cell(row, 1).Value = p.Id;
            sheet.Cell(row, 2).Value = p.Name;
            sheet.Cell(row, 3).Value = p.Price;
            sheet.Cell(row, 4).Value = p.Stock;
            sheet.Cell(row, 5).Value = p.Category;
            sheet.Cell(row, 6).Value = p.IsActive ? "啟用" : "停用";
            row++;
        }

        var dataRange = sheet.Range(1, 1, row - 1, 6);
        dataRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        dataRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

        sheet.Column(3).Style.NumberFormat.Format = "#,##0.00";   // 售價
        sheet.Columns().AdjustToContents();

        //// 中文欄位補最小寬度（AdjustToContents 對中文估寬偏窄）
        //SetMinWidth(sheet.Column(2), 18);   // 商品名稱
        //SetMinWidth(sheet.Column(5), 10);   // 分類
        //SetMinWidth(sheet.Column(6), 8);    // 狀態
        EnsureHeaderWidth(sheet, 6);   // 6 欄，確保標題不被截

        // 中文「資料」欄再補寬（標題以外的內容顯示）
        SetMinWidth(sheet.Column(2), 18);   // 商品名稱
        SetMinWidth(sheet.Column(5), 10);   // 分類
        workbook.SaveAs(filePath);
    }

    // ── 匯出進貨明細表 ─────────────────────────────────
    public static void ExportPurchaseOrders(List<PurchaseOrderDto> orders, string filePath)
    {
        using var workbook = new XLWorkbook();
        var sheet = workbook.Worksheets.Add("進貨明細");

        sheet.Cell(1, 1).Value = "單號";
        sheet.Cell(1, 2).Value = "進貨日期";
        sheet.Cell(1, 3).Value = "供應商";
        sheet.Cell(1, 4).Value = "商品名稱";
        sheet.Cell(1, 5).Value = "數量";
        sheet.Cell(1, 6).Value = "單價";
        sheet.Cell(1, 7).Value = "小計";
        sheet.Cell(1, 8).Value = "狀態";
        sheet.Cell(1, 9).Value = "建立者";

        var headerRange = sheet.Range(1, 1, 1, 9);
        headerRange.Style.Font.Bold = true;
        headerRange.Style.Fill.BackgroundColor = XLColor.LightGreen;
        headerRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

        int row = 2;
        foreach (var o in orders)
        {
            foreach (var d in o.Details)
            {
                sheet.Cell(row, 1).Value = o.Id;
                sheet.Cell(row, 2).Value = o.OrderDate.ToLocalTime().ToString("yyyy/MM/dd");
                sheet.Cell(row, 3).Value = o.Supplier;
                sheet.Cell(row, 4).Value = d.ProductName;
                sheet.Cell(row, 5).Value = d.Quantity;
                sheet.Cell(row, 6).Value = d.UnitPrice;
                sheet.Cell(row, 7).Value = d.Subtotal;
                sheet.Cell(row, 8).Value = o.Status == "Posted" ? "正常" : "已作廢";
                sheet.Cell(row, 9).Value = o.CreatedBy;
                row++;
            }
        }

        var dataRange = sheet.Range(1, 1, row - 1, 9);
        dataRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        dataRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

        sheet.Column(6).Style.NumberFormat.Format = "#,##0.00";
        sheet.Column(7).Style.NumberFormat.Format = "#,##0.00";
        sheet.Columns().AdjustToContents();

        //EnsureHeaderWidth(sheet, 9);   // 9 欄，確保標題不被截
        //SetMinWidth(sheet.Column(2), 12);   // 進貨日期
        //SetMinWidth(sheet.Column(3), 16);   // 供應商
        //SetMinWidth(sheet.Column(4), 18);   // 商品名稱
        //SetMinWidth(sheet.Column(8), 8);    // 狀態
        //SetMinWidth(sheet.Column(9), 10);   // 建立者
        EnsureHeaderWidth(sheet, 9);   // 9 欄

        SetMinWidth(sheet.Column(3), 16);   // 供應商
        SetMinWidth(sheet.Column(4), 18);   // 商品名稱
        workbook.SaveAs(filePath);
    }

    // ── 匯出銷貨明細表（結構同進貨，供應商→客戶）──
    public static void ExportSalesOrders(List<SalesOrderDto> orders, string filePath)
    {
        using var workbook = new XLWorkbook();
        var sheet = workbook.Worksheets.Add("銷貨明細");

        sheet.Cell(1, 1).Value = "單號";
        sheet.Cell(1, 2).Value = "銷貨日期";
        sheet.Cell(1, 3).Value = "客戶";
        sheet.Cell(1, 4).Value = "商品名稱";
        sheet.Cell(1, 5).Value = "數量";
        sheet.Cell(1, 6).Value = "單價";
        sheet.Cell(1, 7).Value = "小計";
        sheet.Cell(1, 8).Value = "狀態";
        sheet.Cell(1, 9).Value = "建立者";

        var headerRange = sheet.Range(1, 1, 1, 9);
        headerRange.Style.Font.Bold = true;
        headerRange.Style.Fill.BackgroundColor = XLColor.LightYellow;
        headerRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

        int row = 2;
        foreach (var o in orders)
        {
            foreach (var d in o.Details)
            {
                sheet.Cell(row, 1).Value = o.Id;
                sheet.Cell(row, 2).Value = o.OrderDate.ToLocalTime().ToString("yyyy/MM/dd");
                sheet.Cell(row, 3).Value = o.Customer;
                sheet.Cell(row, 4).Value = d.ProductName;
                sheet.Cell(row, 5).Value = d.Quantity;
                sheet.Cell(row, 6).Value = d.UnitPrice;
                sheet.Cell(row, 7).Value = d.Subtotal;
                sheet.Cell(row, 8).Value = o.Status == "Posted" ? "正常" : "已作廢";
                sheet.Cell(row, 9).Value = o.CreatedBy;
                row++;
            }
        }

        var dataRange = sheet.Range(1, 1, row - 1, 9);
        dataRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        dataRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

        sheet.Column(6).Style.NumberFormat.Format = "#,##0.00";
        sheet.Column(7).Style.NumberFormat.Format = "#,##0.00";
        sheet.Columns().AdjustToContents();

        //SetMinWidth(sheet.Column(2), 12);   // 銷貨日期
        //SetMinWidth(sheet.Column(3), 16);   // 客戶
        //SetMinWidth(sheet.Column(4), 18);   // 商品名稱
        //SetMinWidth(sheet.Column(8), 8);    // 狀態
        //SetMinWidth(sheet.Column(9), 10);   // 建立者
        EnsureHeaderWidth(sheet, 9);   // 9 欄

        SetMinWidth(sheet.Column(3), 16);   // 客戶
        SetMinWidth(sheet.Column(4), 18);   // 商品名稱
        workbook.SaveAs(filePath);
    }
    // ── 若欄寬小於最小值，補到最小值（改善中文顯示）──
    private static void SetMinWidth(IXLColumn column, double minWidth)
    {
        if (column.Width < minWidth)
            column.Width = minWidth;
    }
    // ── 確保每一欄都容得下「標題文字」寬度（中文算兩倍寬）──
    private static void EnsureHeaderWidth(IXLWorksheet sheet, int lastColumn)
    {
        for (int col = 1; col <= lastColumn; col++)
        {
            var headerText = sheet.Cell(1, col).GetString();
            // 中文字算 2、其餘算 1，再加一點緩衝
            double needed = 0;
            foreach (var ch in headerText)
                needed += ch > 127 ? 2 : 1;
            needed += 2;   // 緩衝

            if (sheet.Column(col).Width < needed)
                sheet.Column(col).Width = needed;
        }
    }
}