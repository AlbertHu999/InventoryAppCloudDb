// DTOs/InventoryLedgerDto.cs
namespace InventoryAppCloudDb.Api.DTOs;

public class InventoryLedgerDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = "";
    public string SourceType { get; set; } = "";
    public int SourceOrderId { get; set; }
    public string Direction { get; set; } = "";
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; } = "";
    public string? Remark { get; set; }
}