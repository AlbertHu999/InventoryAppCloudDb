
namespace InventoryAppCloudDb.Api.Services;

using InventoryAppCloudDb.Api.DTOs;
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

    public async Task<ServiceResult<LoginResponseDto>> LoginAsync(string username, string password)
    {
        var user = await _ctx.Users.FirstOrDefaultAsync(u => u.Username == username);
        if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            return ServiceResult<LoginResponseDto>.Fail("帳號或密碼錯誤");

        var token = Guid.NewGuid().ToString("N");
        _ctx.UserTokens.Add(new UserToken
        {
            UserId = user.Id,
            Token = token,
            ExpiresAt = DateTime.UtcNow.AddHours(8),
        });
        await _ctx.SaveChangesAsync();

        return ServiceResult<LoginResponseDto>.Ok(new LoginResponseDto
        {
            Token = token,
            Username = user.Username,
            Role = user.Role,
        });
    }
    public async Task<(bool IsValid, string Role, string Username)> ValidateTokenAsync(string token)
    {
        var userToken = await _ctx.UserTokens
                    .Include(t => t.User)
                    .FirstOrDefaultAsync(t => t.Token == token
                                           && t.ExpiresAt > DateTime.UtcNow
                                           && t.RevokedAt == null);   // ← 新增：排除已撤銷的 Token
        if (userToken == null) return (false, "", "");
        return (true, userToken.User!.Role, userToken.User!.Username);
    }

    // ── Phase 5.5 Day43-44：登出（撤銷 Token）──
    public async Task<ServiceResult> LogoutAsync(string token)
    {
        var userToken = await _ctx.UserTokens
            .FirstOrDefaultAsync(t => t.Token == token);

        if (userToken == null)
            return ServiceResult.Ok();   // Token 不存在也視為登出成功，不需報錯

        userToken.RevokedAt = DateTime.UtcNow;
        await _ctx.SaveChangesAsync();
        return ServiceResult.Ok();
    }
}