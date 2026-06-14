using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryAppCloudDb.Api.Models;

[Table("sales_order_details")]
public class SalesOrderDetail
{
    [Column("id")]
    public int Id { get; set; }

    [Column("sales_order_id")]
    public int SalesOrderId { get; set; }

    [Column("product_id")]
    public int ProductId { get; set; }

    [Column("quantity")]
    public int Quantity { get; set; }

    [Column("unit_price")]
    public decimal UnitPrice { get; set; }

    public SalesOrder? SalesOrder { get; set; }
    public Product? Product { get; set; }
}