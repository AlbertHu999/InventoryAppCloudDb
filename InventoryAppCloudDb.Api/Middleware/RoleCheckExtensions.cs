// Middleware/RoleCheckExtensions.cs
namespace InventoryAppCloudDb.Api.Middleware;

public static class RoleCheckExtensions
{
    // 檢查目前請求是否為 Admin
    public static bool IsAdmin(this HttpContext ctx)
        => ctx.Items["UserRole"]?.ToString() == "Admin";
}