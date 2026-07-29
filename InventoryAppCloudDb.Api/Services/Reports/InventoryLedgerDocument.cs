// Services/Reports/InventoryLedgerDocument.cs
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using InventoryAppCloudDb.Api.DTOs;

namespace InventoryAppCloudDb.Api.Services.Reports;

public class InventoryLedgerDocument : IDocument
{
    private readonly List<InventoryLedgerDto> _ledgers;

    public InventoryLedgerDocument(List<InventoryLedgerDto> ledgers) => _ledgers = ledgers;

    public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

    public void Compose(IDocumentContainer container)
    {
        container.Page(page =>
        {
            page.Size(PageSizes.A4.Landscape());
            page.Margin(30);
            page.DefaultTextStyle(x => x.FontFamily("NotoSansTC").FontSize(9));

            page.Header().Text("庫存異動明細表").FontSize(18).Bold().FontColor(Colors.Cyan.Darken2);

            page.Content().PaddingVertical(10).Table(table =>
            {
                table.ColumnsDefinition(cols =>
                {
                    cols.RelativeColumn(2);
                    cols.RelativeColumn(2);
                    cols.RelativeColumn(1.5f);
                    cols.RelativeColumn(1);
                    cols.RelativeColumn(1);
                    cols.RelativeColumn(1.5f);
                    cols.RelativeColumn(1.5f);
                    cols.RelativeColumn(3);
                });

                table.Header(header =>
                {
                    header.Cell().Text("時間").Bold();
                    header.Cell().Text("商品").Bold();
                    header.Cell().Text("來源").Bold();
                    header.Cell().Text("方向").Bold();
                    header.Cell().Text("數量").Bold();
                    header.Cell().Text("單價").Bold();
                    header.Cell().Text("操作者").Bold();
                    header.Cell().Text("備註").Bold();
                });

                foreach (var l in _ledgers)
                {
                    var directionText = l.Direction == "In" ? "▲入庫" : "▼出庫";
                    var directionColor = l.Direction == "In" ? Colors.Green.Darken1 : Colors.Red.Darken1;

                    table.Cell().Text(l.CreatedAt.ToLocalTime().ToString("MM/dd HH:mm"));
                    table.Cell().Text(l.ProductName);
                    table.Cell().Text(TranslateSourceType(l.SourceType));
                    table.Cell().Text(directionText).FontColor(directionColor);
                    table.Cell().Text(l.Quantity.ToString());
                    table.Cell().Text(l.UnitPrice.ToString("N2"));
                    table.Cell().Text(l.CreatedBy);
                    table.Cell().Text(l.Remark ?? "");
                }
            });

            page.Footer().AlignCenter().Text(text =>
            {
                text.Span("列印時間：");
                text.Span(DateTime.Now.ToString("yyyy/MM/dd HH:mm"));
            });
        });
    }

    private static string TranslateSourceType(string sourceType) => sourceType switch
    {
        "Purchase" => "進貨",
        "Sales" => "銷貨",
        "PurchaseVoid" => "進貨作廢",
        "SalesVoid" => "銷貨作廢",
        "Adjust" => "調整",
        _ => sourceType,
    };
}