
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

    // 導覽屬性
    public User? User { get; set; }
}