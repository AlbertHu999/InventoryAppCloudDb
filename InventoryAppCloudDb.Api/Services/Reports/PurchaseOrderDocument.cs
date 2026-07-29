// Services/Reports/PurchaseOrderDocument.cs
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using InventoryAppCloudDb.Api.DTOs;

namespace InventoryAppCloudDb.Api.Services.Reports;

public class PurchaseOrderDocument : IDocument
{
    private readonly PurchaseOrderDto _order;

    public PurchaseOrderDocument(PurchaseOrderDto order) => _order = order;

    public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

    public void Compose(IDocumentContainer container)
    {
        container.Page(page =>
        {
            page.Size(PageSizes.A4);
            page.Margin(30);
            page.DefaultTextStyle(x => x.FontFamily("NotoSansTC").FontSize(10));

            page.Header().Column(col =>
            {
                col.Item().Text("進貨單").FontSize(18).Bold().FontColor(Colors.Green.Darken2);
                col.Item().Text($"單號：{_order.Id}　狀態：{(_order.Status == "Posted" ? "正常" : "已作廢")}");
                col.Item().Text($"供應商：{_order.Supplier}　進貨日期：{_order.OrderDate:yyyy/MM/dd}");
                col.Item().Text($"建立者：{_order.CreatedBy}");
            });

            page.Content().PaddingVertical(10).Table(table =>
            {
                table.ColumnsDefinition(cols =>
                {
                    cols.RelativeColumn(3);
                    cols.RelativeColumn(1.5f);
                    cols.RelativeColumn(2);
                    cols.RelativeColumn(2);
                });

                table.Header(header =>
                {
                    header.Cell().Text("商品名稱").Bold();
                    header.Cell().Text("數量").Bold();
                    header.Cell().Text("單價").Bold();
                    header.Cell().Text("小計").Bold();
                });

                decimal total = 0;
                foreach (var d in _order.Details)
                {
                    var subtotal = d.Quantity * d.UnitPrice;
                    total += subtotal;

                    table.Cell().Text(d.ProductName);
                    table.Cell().Text(d.Quantity.ToString());
                    table.Cell().Text(d.UnitPrice.ToString("N2"));
                    table.Cell().Text(subtotal.ToString("N2"));
                }

                table.Cell().ColumnSpan(3).AlignRight().Text("合計：").Bold();
                table.Cell().Text(total.ToString("N2")).Bold();
            });
        });
    }
}