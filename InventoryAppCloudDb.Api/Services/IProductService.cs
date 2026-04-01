using InventoryAppCloudDb.Api.DTOs;

namespace InventoryAppCloudDb.Api.Services;

public interface IProductService
{
    List<ProductDto> GetAll();
    ProductDto? GetById(int id);
    List<ProductDto> GetByCategory(string category);
    ProductDto? Create(CreateProductDto dto);
    bool Update(int id, UpdateProductDto dto);
    bool Delete(int id);
}