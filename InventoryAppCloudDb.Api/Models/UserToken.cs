
namespace InventoryAppCloudDb.Api.Models;

using System.ComponentModel.DataAnnotations.Schema;

// Models/UserToken.cs
[Table("user_tokens")]
public class UserToken
{
    [Column("id")]
    public int Id { get; set; }

    [Column("user_id")]
    public int UserId { get; set; }

    [Column("token")]
    public string Token { get; set; } = "";

    [Column("expires_at")]
    public DateTime ExpiresAt { get; set; }

    // ── Phase 5.5 新增 ──
    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;   // Token 發放時間

    [Column("revoked_at")]
    public DateTime? RevokedAt { get; set; }   // Token 撤銷時間（登出時賦值）

    // 導覽屬性
    public User? User { get; set; }
}