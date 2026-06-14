namespace InventoryAppCloudDb.DTOs;

// ── 銷貨單主表（查詢用）──────────────────────────────────
public class SalesOrderDto
{
    public int Id { get; set; }
    public string OrderNo { get; set; } = "";
    public string Customer { get; set; } = "";
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = "";
    public string CreatedBy { get; set; } = "";
    public DateTime CreatedAt { get; set; }
    public List<SalesOrderDetailDto> Details { get; set; } = new();
}

// ── 銷貨單明細（查詢用）──────────────────────────────────
public class SalesOrderDetailDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = "";
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Subtotal { get; set; }
}

// ── 建立銷貨單（新增用，傳給 API）────────────────────────
public class CreateSalesOrderDto
{
    public string Customer { get; set; } = "";
    public List<CreateSalesDetailDto> Details { get; set; } = new();
}

// ── 建立銷貨明細（新增用）────────────────────────────────
public class CreateSalesDetailDto
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}