
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