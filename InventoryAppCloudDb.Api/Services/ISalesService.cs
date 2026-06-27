using InventoryAppCloudDb.Api.DTOs;
using InventoryAppCloudDb.Api.Models;

namespace InventoryAppCloudDb.Api.Services;

public interface ISalesService
{
    Task<ServiceResult<List<SalesOrderDto>>> GetAllAsync();
    Task<ServiceResult<SalesOrderDto>> GetByIdAsync(int id);
    Task<ServiceResult<SalesOrderDto>> CreateAsync(
        CreateSalesOrderDto dto, string createdBy);
    // ── Phase 5.5 Day43-44 新增：作廢 ──
    Task<ServiceResult<SalesOrderDto>> VoidAsync(
        int id, string reason, string voidedBy);
}