// Services/Reports/InventoryLedgerPdfService.cs
using InventoryAppCloudDb.Api.DTOs;
using QuestPDF.Fluent;

namespace InventoryAppCloudDb.Api.Services.Reports;

public static class InventoryLedgerPdfService
{
    public static byte[] Generate(List<InventoryLedgerDto> ledgers)
    {
        var document = new InventoryLedgerDocument(ledgers);
        return document.GeneratePdf();
    }
}