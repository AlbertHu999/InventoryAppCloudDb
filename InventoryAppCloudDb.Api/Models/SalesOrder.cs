using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryAppCloudDb.Api.Models;

[Table("sales_orders")]
public class SalesOrder
{
    [Column("id")]
    public int Id { get; set; }

    [Column("order_date")]
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;

    [Column("customer")]
    [MaxLength(100)]
    public string Customer { get; set; } = "";

    [Column("note")]
    [MaxLength(200)]
    public string Note { get; set; } = "";

    [Column("created_by")]
    [MaxLength(50)]
    public string CreatedBy { get; set; } = "";

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public List<SalesOrderDetail> Details { get; set; } = [];

    // ── Phase 5.5 新增：單據狀態與作廢欄位 ──
    [Column("status")]
    [MaxLength(20)]
    public string Status { get; set; } = "Posted";   // Posted 已過帳 / Voided 已作廢

    [Column("voided_at")]
    public DateTime? VoidedAt { get; set; }           // 作廢時間（可為 null）

    [Column("voided_by")]
    [MaxLength(50)]
    public string? VoidedBy { get; set; }             // 作廢人

    [Column("void_reason")]
    [MaxLength(200)]
    public string? VoidReason { get; set; }           // 作廢原因
}