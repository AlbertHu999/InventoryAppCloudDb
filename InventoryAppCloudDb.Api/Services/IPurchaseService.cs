using InventoryAppCloudDb.Api.DTOs;
using InventoryAppCloudDb.Api.Models;

namespace InventoryAppCloudDb.Api.Services;

public interface IPurchaseService
{
    Task<ServiceResult<List<PurchaseOrderDto>>> GetAllAsync();
    Task<ServiceResult<PurchaseOrderDto>> GetByIdAsync(int id);
    Task<ServiceResult<PurchaseOrderDto>> CreateAsync(
        CreatePurchaseOrderDto dto, string createdBy);
}