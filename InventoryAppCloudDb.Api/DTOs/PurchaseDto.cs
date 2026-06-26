namespace InventoryAppCloudDb.Api.DTOs;

public class PurchaseDetailDto
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}

public class CreatePurchaseOrderDto
{
    public string Supplier { get; set; } = "";
    public string Note { get; set; } = "";
    public List<PurchaseDetailDto> Details { get; set; } = [];
}

public class PurchaseDetailResponseDto
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = "";
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Subtotal => Quantity * UnitPrice;
}

public class PurchaseOrderDto
{
    public int Id { get; set; }
    public DateTime OrderDate { get; set; }
    public string Supplier { get; set; } = "";
    public string Note { get; set; } = "";
    public string CreatedBy { get; set; } = "";
    public string Status { get; set; } = "";   // ← Phase 5.5 新增（Posted / Voided）
    public List<PurchaseDetailResponseDto> Details { get; set; } = [];
}