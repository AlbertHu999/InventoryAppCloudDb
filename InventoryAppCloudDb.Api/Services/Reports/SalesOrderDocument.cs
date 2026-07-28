// Services/Reports/SalesOrderDocument.cs
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using InventoryAppCloudDb.Api.DTOs;

namespace InventoryAppCloudDb.Api.Services.Reports;

// ── 銷貨單 PDF 版面定義（純 C# 描述版面，不需要拖拉設計器）──
public class SalesOrderDocument : IDocument
{
    private readonly SalesOrderDto _order;
    private const string FontFamily = "NotoSansTC";

    public SalesOrderDocument(SalesOrderDto order)
    {
        _order = order;
    }

    public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

    public void Compose(IDocumentContainer container)
    {
        container.Page(page =>
        {
            page.Size(PageSizes.A4);
            page.Margin(30);
            page.DefaultTextStyle(x => x.FontFamily(FontFamily).FontSize(11));

            page.Header().Element(ComposeHeader);
            page.Content().Element(ComposeContent);
            page.Footer().AlignRight().Text(text =>
            {
                text.Span("列印時間：").FontSize(9).FontColor(Colors.Grey.Darken1);
                text.Span($"{DateTime.Now:yyyy/MM/dd HH:mm}").FontSize(9).FontColor(Colors.Grey.Darken1);
            });
        });
    }

    private void ComposeHeader(IContainer container)
    {
        container.Column(column =>
        {
            column.Item().AlignCenter().Text("銷貨單").FontSize(22).Bold();
            column.Item().AlignCenter().Text("Sales Order").FontSize(11).FontColor(Colors.Grey.Darken1);

            column.Item().PaddingTop(15).Row(row =>
            {
                row.RelativeItem().Column(c =>
                {
                    c.Item().Text($"單號：{_order.Id}");
                    c.Item().Text($"客戶：{_order.Customer}");
                });
                row.RelativeItem().Column(c =>
                {
                    c.Item().Text($"日期：{_order.OrderDate.ToLocalTime():yyyy/MM/dd}");
                    c.Item().Text($"建立者：{_order.CreatedBy}");
                });
            });

            column.Item().PaddingTop(8).LineHorizontal(1).LineColor(Colors.Grey.Medium);
        });
    }

    private void ComposeContent(IContainer container)
    {
        container.PaddingTop(15).Column(column =>
        {
            column.Item().Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(1);
                    columns.RelativeColumn(4);
                    columns.RelativeColumn(2);
                    columns.RelativeColumn(2.5f);
                    columns.RelativeColumn(2.5f);
                });

                table.Header(header =>
                {
                    header.Cell().Element(HeaderCellStyle).Text("項次");
                    header.Cell().Element(HeaderCellStyle).Text("商品名稱");
                    header.Cell().Element(HeaderCellStyle).AlignRight().Text("數量");
                    header.Cell().Element(HeaderCellStyle).AlignRight().Text("單價");
                    header.Cell().Element(HeaderCellStyle).AlignRight().Text("小計");

                    static IContainer HeaderCellStyle(IContainer c) => c
                        .DefaultTextStyle(x => x.Bold())
                        .PaddingVertical(6).PaddingHorizontal(4)
                        .Background(Colors.Grey.Lighten3)
                        .BorderBottom(1).BorderColor(Colors.Grey.Darken1);
                });

                int seq = 1;
                foreach (var d in _order.Details)
                {
                    table.Cell().Element(BodyCellStyle).Text(seq.ToString());
                    table.Cell().Element(BodyCellStyle).Text(d.ProductName);
                    table.Cell().Element(BodyCellStyle).AlignRight().Text(d.Quantity.ToString());
                    table.Cell().Element(BodyCellStyle).AlignRight().Text(d.UnitPrice.ToString("N2"));
                    table.Cell().Element(BodyCellStyle).AlignRight().Text(d.Subtotal.ToString("N2"));
                    seq++;

                    static IContainer BodyCellStyle(IContainer c) => c
                        .PaddingVertical(5).PaddingHorizontal(4)
                        .BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten1);
                }
            });

            column.Item().PaddingTop(10).AlignRight().Row(row =>
            {
                row.ConstantItem(100).Text("總金額：").Bold().FontSize(13);
                row.ConstantItem(120).AlignRight().Text($"NT$ {_order.TotalAmount:N2}")
                    .Bold().FontSize(13);
            });
        });
    }
}