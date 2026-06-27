

namespace InventoryAppCloudDb.Api.Services;

using InventoryAppCloudDb.Api.DTOs;
using InventoryAppCloudDb.Api.Models;
// Services/IAuthService.cs
// Services/IAuthService.cs
public interface IAuthService
{
    Task<ServiceResult<LoginResponseDto>> LoginAsync(string username, string password);
    Task<(bool IsValid, string Role, string Username)> ValidateTokenAsync(string token);
    Task<ServiceResult> LogoutAsync(string token);
}