// Services/Reports/SalesOrderPdfService.cs
using QuestPDF.Fluent;
using InventoryAppCloudDb.Api.DTOs;

namespace InventoryAppCloudDb.Api.Services.Reports;

// ── 把 SalesOrderDto 轉成 PDF bytes（跟 ExcelExportService 同性質的靜態工具類別）──
public static class SalesOrderPdfService
{
    public static byte[] Generate(SalesOrderDto order)
    {
        var document = new SalesOrderDocument(order);
        return document.GeneratePdf();
    }
}