// Models/InventoryLedger.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryAppCloudDb.Api.Models;

[Table("inventory_ledgers")]
public class InventoryLedger
{
    [Column("id")]
    public int Id { get; set; }

    [Column("product_id")]
    public int ProductId { get; set; }

    // Purchase / Sales / PurchaseVoid / SalesVoid / Adjust
    [Column("source_type")]
    [MaxLength(30)]
    public string SourceType { get; set; } = "";

    [Column("source_order_id")]
    public int SourceOrderId { get; set; }

    [Column("source_detail_id")]
    public int? SourceDetailId { get; set; }

    // In / Out
    [Column("direction")]
    [MaxLength(10)]
    public string Direction { get; set; } = "";

    [Column("quantity")]
    public int Quantity { get; set; }   // 永遠存正數

    [Column("unit_price")]
    public decimal UnitPrice { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("created_by")]
    [MaxLength(50)]
    public string CreatedBy { get; set; } = "";

    [Column("remark")]
    [MaxLength(200)]
    public string? Remark { get; set; }

    // 導覽屬性
    public Product? Product { get; set; }
}