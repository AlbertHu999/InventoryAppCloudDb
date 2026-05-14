

namespace InventoryAppCloudDb.Api.Services;

using InventoryAppCloudDb.Api.Models;
// Services/IAuthService.cs
// Services/IAuthService.cs
public interface IAuthService
{
    Task<ServiceResult<string>> LoginAsync(string username, string password);
    Task<(bool IsValid, string Role)> ValidateTokenAsync(string token);
}