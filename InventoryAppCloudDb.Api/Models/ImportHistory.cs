using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryAppCloudDb.Api.Models;

[Table("import_histories")]
public class ImportHistory
{
    [Column("id")]
    public int Id { get; set; }

    [Column("file_hash")]
    [MaxLength(64)]   // SHA256 十六進位字串固定 64 字元
    public string FileHash { get; set; } = "";

    [Column("file_name")]
    [MaxLength(255)]
    public string FileName { get; set; } = "";

    [Column("sheet_type")]
    [MaxLength(20)]   // 商品 / 進貨 / 銷貨
    public string SheetType { get; set; } = "";

    [Column("imported_at")]
    public DateTime ImportedAt { get; set; } = DateTime.UtcNow;

    [Column("imported_by")]
    [MaxLength(50)]
    public string ImportedBy { get; set; } = "";
}