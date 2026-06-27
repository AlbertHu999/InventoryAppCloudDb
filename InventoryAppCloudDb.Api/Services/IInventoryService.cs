// Services/IInventoryService.cs
using InventoryAppCloudDb.Api.DTOs;
using InventoryAppCloudDb.Api.Models;

namespace InventoryAppCloudDb.Api.Services;

public interface IInventoryService
{
    Task<ServiceResult<List<InventoryLedgerDto>>> GetAllAsync();
    Task<ServiceResult<List<InventoryLedgerDto>>> GetByProductIdAsync(int productId);
}