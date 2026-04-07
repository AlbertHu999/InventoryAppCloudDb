using InventoryAppCloudDb.Api.Models;

namespace InventoryAppCloudDb.Api.Repositories;

public interface IProductRepository
{
    Task<List<Product>> GetAllAsync();
    Task<Product?> GetByIdAsync(int id);
    Task<List<Product>> GetByCategoryAsync(string category);
    Task<int> InsertAsync(Product product);
    Task<bool> UpdateAsync(Product product);
    Task<bool> DeleteAsync(int id);
}