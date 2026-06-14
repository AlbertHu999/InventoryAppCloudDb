namespace InventoryAppCloudDb.Api.DTOs;

public class SalesDetailDto
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}

public class CreateSalesOrderDto
{
    public string Customer { get; set; } = "";
    public string Note { get; set; } = "";
    public List<SalesDetailDto> Details { get; set; } = [];
}

public class SalesDetailResponseDto
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = "";
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Subtotal => Quantity * UnitPrice;
}

public class SalesOrderDto
{
    public int Id { get; set; }
    public DateTime OrderDate { get; set; }
    public string Customer { get; set; } = "";
    public string Note { get; set; } = "";
    public string CreatedBy { get; set; } = "";
    public List<SalesDetailResponseDto> Details { get; set; } = [];
    public decimal TotalAmount => Details.Sum(d => d.Subtotal);
}