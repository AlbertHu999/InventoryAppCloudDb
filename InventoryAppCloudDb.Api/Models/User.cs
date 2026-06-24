namespace InventoryAppCloudDb.Api.Models;
// Models/User.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("users")]
public class User
{
    [Column("id")]
    public int Id { get; set; }

    [Column("username")]
    [MaxLength(50)]
    public string Username { get; set; } = "";

    [Column("password_hash")]
    public string PasswordHash { get; set; } = "";

    [Column("role")]
    [MaxLength(20)]
    public string Role { get; set; } = "User";  // "Admin" 或 "User"

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // ── Phase 5.5 新增 ──
    [Column("is_active")]
    public bool IsActive { get; set; } = true;   // 使用者停用/啟用（離職員工不刪帳，改停用）
}