using InventoryAppCloudDb.Api.DTOs;
using InventoryAppCloudDb.Api.Models;
using InventoryAppCloudDb.Api.Repositories;

namespace InventoryAppCloudDb.Api.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _repo;

    public ProductService(IProductRepository repo)
    {
        _repo = repo;  // DI 注入 Repository
    }

    public List<ProductDto> GetAll()
    {
        return _repo.GetAll()
            .Select(p => ToDto(p))
            .ToList();
    }

    public ProductDto? GetById(int id)
    {
        var product = _repo.GetById(id);
        return product == null ? null : ToDto(product);
    }

    public List<ProductDto> GetByCategory(string category)
    {
        return _repo.GetByCategory(category)
            .Select(p => ToDto(p))
            .ToList();
    }

    public ProductDto? Create(CreateProductDto dto)
    {
        // ★ 商業邏輯驗證（這些判斷只能放在 Service，不能放在 Repository）
        if (string.IsNullOrWhiteSpace(dto.Name))
            return null;

        if (dto.Price < 0)
            return null;

        if (dto.Stock < 0)
            return null;

        // 轉換 DTO → Entity
        var product = new Product
        {
            Name = dto.Name.Trim(),
            Price = dto.Price,
            Stock = dto.Stock,
            Category = string.IsNullOrWhiteSpace(dto.Category) ? "未分類" : dto.Category.Trim(),
            CreatedAt = DateTime.UtcNow
        };

        int newId = _repo.Insert(product);

        // 回傳新建的完整資料
        return GetById(newId);
    }

    public bool Update(int id, UpdateProductDto dto)
    {
        // ★ 商業邏輯驗證
        if (string.IsNullOrWhiteSpace(dto.Name))
            return false;

        if (dto.Price < 0)
            return false;

        if (dto.Stock < 0)
            return false;

        var product = new Product
        {
            Id = id,
            Name = dto.Name.Trim(),
            Price = dto.Price,
            Stock = dto.Stock,
            Category = string.IsNullOrWhiteSpace(dto.Category) ? "未分類" : dto.Category.Trim()
        };

        return _repo.Update(product);
    }

    public bool Delete(int id)
    {
        return _repo.Delete(id);
    }

    // ★ 私有方法：Entity → DTO 轉換（集中在一處，方便維護）
    private static ProductDto ToDto(Product p)
    {
        return new ProductDto
        {
            Id = p.Id,
            Name = p.Name,
            Price = p.Price,
            Stock = p.Stock,
            Category = p.Category
        };
    }
}
