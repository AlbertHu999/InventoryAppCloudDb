
namespace InventoryAppCloudDb.Api.Services;

using InventoryAppCloudDb.Api.Models;
using Microsoft.EntityFrameworkCore;
// Services/AuthService.cs
public class AuthService : IAuthService
{
    private readonly AppDbContext _ctx;

    public AuthService(AppDbContext ctx)
    {
        _ctx = ctx;
    }

    public async Task<ServiceResult<string>> LoginAsync(string username, string password)
    {
        // 找使用者
        var user = await _ctx.Users
            .FirstOrDefaultAsync(u => u.Username == username);

        if (user == null)
            return ServiceResult<string>.Fail("帳號或密碼錯誤");

        // 驗證密碼（BCrypt）
        if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            return ServiceResult<string>.Fail("帳號或密碼錯誤");

        // 產生 Token（GUID，唯一且難以猜測）
        var token = Guid.NewGuid().ToString("N");

        // 存入資料庫，設定 8 小時後過期
        _ctx.UserTokens.Add(new UserToken
        {
            UserId = user.Id,
            Token = token,
            ExpiresAt = DateTime.UtcNow.AddHours(8),
        });
        await _ctx.SaveChangesAsync();

        return ServiceResult<string>.Ok(token);
    }

    public async Task<(bool IsValid, string Role)> ValidateTokenAsync(string token)
    {
        var userToken = await _ctx.UserTokens
            .Include(t => t.User)
            .FirstOrDefaultAsync(t => t.Token == token
                                   && t.ExpiresAt > DateTime.UtcNow);

        if (userToken == null)
            return (false, "");

        return (true, userToken.User!.Role);
    }
}