// Services/Reports/ProductDocument.cs
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using InventoryAppCloudDb.Api.DTOs;

namespace InventoryAppCloudDb.Api.Services.Reports;

public class ProductDocument : IDocument
{
    private readonly List<ProductDto> _products;

    public ProductDocument(List<ProductDto> products) => _products = products;

    public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

    public void Compose(IDocumentContainer container)
    {
        container.Page(page =>
        {
            page.Size(PageSizes.A4);
            page.Margin(30);
            page.DefaultTextStyle(x => x.FontFamily("NotoSansTC").FontSize(10));

            page.Header().Text("商品庫存表")
                .FontSize(18).Bold().FontColor(Colors.Blue.Darken2);

            page.Content().PaddingVertical(10).Table(table =>
            {
                table.ColumnsDefinition(cols =>
                {
                    cols.ConstantColumn(40);
                    cols.RelativeColumn(3);
                    cols.RelativeColumn(2);
                    cols.RelativeColumn(1.5f);
                    cols.RelativeColumn(2);
                    cols.RelativeColumn(1.5f);
                    cols.RelativeColumn(2);
                });

                table.Header(header =>
                {
                    header.Cell().Text("編號").Bold();
                    header.Cell().Text("商品名稱").Bold();
                    header.Cell().Text("售價").Bold();
                    header.Cell().Text("庫存").Bold();
                    header.Cell().Text("分類").Bold();
                    header.Cell().Text("狀態").Bold();
                    header.Cell().Text("庫存總值").Bold();
                });

                decimal grandTotal = 0;
                foreach (var p in _products)
                {
                    var totalValue = p.Price * p.Stock;
                    grandTotal += totalValue;

                    table.Cell().Text(p.Id.ToString());
                    table.Cell().Text(p.Name);
                    table.Cell().Text(p.Price.ToString("N2"));
                    table.Cell().Text(p.Stock.ToString());
                    table.Cell().Text(p.Category);
                    table.Cell().Text(p.IsActive ? "啟用" : "停用");
                    table.Cell().Text(totalValue.ToString("N2"));
                }

                table.Cell().ColumnSpan(6).AlignRight().Text("庫存總值合計：").Bold();
                table.Cell().Text(grandTotal.ToString("N2")).Bold();
            });

            page.Footer().AlignCenter().Text(text =>
            {
                text.Span("列印時間：");
                text.Span(DateTime.Now.ToString("yyyy/MM/dd HH:mm"));
            });
        });
    }
}