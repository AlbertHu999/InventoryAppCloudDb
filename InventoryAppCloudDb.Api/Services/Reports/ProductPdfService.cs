// Services/Reports/ProductPdfService.cs
using InventoryAppCloudDb.Api.DTOs;
using QuestPDF.Fluent;

namespace InventoryAppCloudDb.Api.Services.Reports;

public static class ProductPdfService
{
    public static byte[] Generate(List<ProductDto> products)
    {
        var document = new ProductDocument(products);
        return document.GeneratePdf();
    }
}