using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryAppCloudDb.Api.Models;

[Table("products")]
public class Product
{
    [Column("id")]
    public int Id { get; set; }

    [Column("name")]
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = "";

    [Column("price")]
    [Range(0, 9999999)]
    public decimal Price { get; set; }

    [Column("stock")]
    public int Stock { get; set; }

    [Column("category")]
    [MaxLength(50)]
    public string Category { get; set; } = "";

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // ── Phase 5.5 新增 ──
    [Column("is_active")]
    public bool IsActive { get; set; } = true;   // 商品停用/啟用（不刪除，改停用）

    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }      // 最後修改時間（可為 null）
}
