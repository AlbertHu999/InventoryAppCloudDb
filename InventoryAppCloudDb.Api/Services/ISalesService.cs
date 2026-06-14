using InventoryAppCloudDb.Api.DTOs;
using InventoryAppCloudDb.Api.Models;

namespace InventoryAppCloudDb.Api.Services;

public interface ISalesService
{
    Task<ServiceResult<List<SalesOrderDto>>> GetAllAsync();
    Task<ServiceResult<SalesOrderDto>> GetByIdAsync(int id);
    Task<ServiceResult<SalesOrderDto>> CreateAsync(
        CreateSalesOrderDto dto, string createdBy);
}