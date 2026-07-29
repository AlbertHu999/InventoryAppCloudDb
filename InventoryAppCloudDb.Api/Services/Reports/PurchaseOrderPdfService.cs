// Services/Reports/PurchaseOrderPdfService.cs
using InventoryAppCloudDb.Api.DTOs;
using QuestPDF.Fluent;

namespace InventoryAppCloudDb.Api.Services.Reports;

public static class PurchaseOrderPdfService
{
    public static byte[] Generate(PurchaseOrderDto order)
    {
        var document = new PurchaseOrderDocument(order);
        return document.GeneratePdf();
    }
}