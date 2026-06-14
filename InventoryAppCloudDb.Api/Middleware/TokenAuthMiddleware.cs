using InventoryAppCloudDb.Api.Models;
using InventoryAppCloudDb.Api.Services;

namespace InventoryAppCloudDb.Api.Middleware;

// Middleware/TokenAuthMiddleware.cs
public class TokenAuthMiddleware
{
    private readonly RequestDelegate _next;

    // 不需要驗證的路徑白名單
    private static readonly string[] _publicPaths =
    [
        "/api/auth/login",
        "/openapi",
        "/swagger",
    ];

    public TokenAuthMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext ctx, IAuthService authService)
    {
        var path = ctx.Request.Path.Value ?? "";

        // 白名單路徑直接放行
        if (_publicPaths.Any(p => path.StartsWith(p, StringComparison.OrdinalIgnoreCase)))
        {
            await _next(ctx);
            return;
        }

        // 取得 Token（從 Header 的 Authorization: Bearer xxx）
        var authHeader = ctx.Request.Headers["Authorization"].FirstOrDefault();
        var token = authHeader?.StartsWith("Bearer ") == true
            ? authHeader["Bearer ".Length..].Trim()
            : null;

        if (string.IsNullOrEmpty(token))
        {
            ctx.Response.StatusCode = 401;
            ctx.Response.ContentType = "application/json";
            await ctx.Response.WriteAsJsonAsync(
                ServiceResult.Fail("未提供 Token，請先登入"));
            return;
        }

        var (isValid, role, username) = await authService.ValidateTokenAsync(token);

        if (!isValid)
        {
            ctx.Response.StatusCode = 401;
            ctx.Response.ContentType = "application/json";
            await ctx.Response.WriteAsJsonAsync(
                ServiceResult.Fail("Token 無效或已過期，請重新登入"));
            return;
        }

        // 把角色和使用者名稱存進 HttpContext
        ctx.Items["UserRole"] = role;
        ctx.Items["Username"] = username;   // ← 新增這行
        await _next(ctx);
    }
}