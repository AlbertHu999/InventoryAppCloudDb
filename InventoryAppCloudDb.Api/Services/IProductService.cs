using InventoryAppCloudDb.Api.DTOs;
using InventoryAppCloudDb.Api.Models;

namespace InventoryAppCloudDb.Api.Services;

public interface IProductService
{
    Task<ServiceResult<List<ProductDto>>> GetAllAsync();
    Task<ServiceResult<ProductDto>> GetByIdAsync(int id);
    Task<ServiceResult<List<ProductDto>>> GetByCategoryAsync(string category);
    Task<ServiceResult<ProductDto>> CreateAsync(CreateProductDto dto);
    Task<ServiceResult<ProductDto>> UpdateAsync(int id, UpdateProductDto dto);
    Task<ServiceResult> DeleteAsync(int id);
}