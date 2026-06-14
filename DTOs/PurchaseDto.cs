namespace InventoryAppCloudDb.DTOs;

// ── 進貨單主表（查詢用）──────────────────────────────────
public class PurchaseOrderDto
{
    public int Id { get; set; }
    public string Supplier { get; set; } = "";
    public string Note { get; set; } = "";
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = "";
    public string CreatedBy { get; set; } = "";
    public DateTime OrderDate { get; set; }
    public List<PurchaseOrderDetailDto> Details { get; set; } = new();
}

// ── 進貨單明細（查詢用）──────────────────────────────────
public class PurchaseOrderDetailDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = "";
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }// ← 對齊 API 的 unitPrice
    public decimal Subtotal { get; set; }
}

// ── 建立進貨單（新增用，傳給 API）────────────────────────
public class CreatePurchaseOrderDto
{
    public string Supplier { get; set; } = "";
    public List<CreatePurchaseDetailDto> Details { get; set; } = new();
}

// ── 建立進貨明細（新增用）────────────────────────────────
public class CreatePurchaseDetailDto
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitCost { get; set; }
}