using InventoryAppCloudDb.Api.Models;

namespace InventoryAppCloudDb.Api.Repositories;

public interface IProductRepository
{
    List<Product> GetAll();
    Product? GetById(int id);
    List<Product> GetByCategory(string category);
    int Insert(Product product);
    bool Update(Product product);
    bool Delete(int id);
}