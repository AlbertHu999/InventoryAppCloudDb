using System;
using System.Collections.Generic;
using System.Text;

namespace InventoryAppLocalDb_EF;

// IProductRepository.cs（介面，之前定義好的）
public interface IProductRepository
{
    List<Product> GetAll();
    Product? GetById(int id);
    List<Product> GetByCategory(string category);
    int Insert(Product product);
    bool Update(Product product);
    bool Delete(int id);
}