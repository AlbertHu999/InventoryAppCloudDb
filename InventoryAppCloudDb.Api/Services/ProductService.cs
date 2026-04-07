using InventoryAppCloudDb.Api.DTOs;
using InventoryAppCloudDb.Api.Models;
using InventoryAppCloudDb.Api.Repositories;

namespace InventoryAppCloudDb.Api.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _repo;

    public ProductService(IProductRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<ProductDto>> GetAllAsync()
    {
        var products = await _repo.GetAllAsync();
        return products.Select(p => ToDto(p)).ToList();
    }

    public async Task<ProductDto?> GetByIdAsync(int id)
    {
        var product = await _repo.GetByIdAsync(id);
        return product == null ? null : ToDto(product);
    }

    public async Task<List<ProductDto>> GetByCategoryAsync(string category)
    {
        var products = await _repo.GetByCategoryAsync(category);
        return products.Select(p => ToDto(p)).ToList();
    }

    public async Task<ProductDto?> CreateAsync(CreateProductDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            return null;

        if (dto.Price < 0)
            return null;

        if (dto.Stock < 0)
            return null;

        var product = new Product
        {
            Name = dto.Name.Trim(),
            Price = dto.Price,
            Stock = dto.Stock,
            Category = string.IsNullOrWhiteSpace(dto.Category) ? "未分類" : dto.Category.Trim(),
            CreatedAt = DateTime.UtcNow
        };

        int newId = await _repo.InsertAsync(product);
        return await GetByIdAsync(newId);
    }

    public async Task<bool> UpdateAsync(int id, UpdateProductDto dto)
    {
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

        return await _repo.UpdateAsync(product);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        return await _repo.DeleteAsync(id);
    }

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