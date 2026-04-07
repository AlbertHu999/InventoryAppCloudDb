using InventoryAppCloudDb.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace InventoryAppCloudDb.Api.Repositories;

public class EFProductRepository : IProductRepository
{
    private readonly AppDbContext _ctx;

    // DbContext 由 DI 注入，不自己 new
    public EFProductRepository(AppDbContext ctx)
    {
        _ctx = ctx;
    }

    public async Task<List<Product>> GetAllAsync()
        => await _ctx.Products.OrderBy(p => p.Id).ToListAsync();

    public async Task<Product?> GetByIdAsync(int id)
        => await _ctx.Products.FindAsync(id);

    public async Task<List<Product>> GetByCategoryAsync(string category)
    {
        return await _ctx.Products
            .Where(p => p.Category == category)
            .OrderBy(p => p.Id)
            .ToListAsync();
    }

    public async Task<int> InsertAsync(Product product)
    {
        _ctx.Products.Add(product);
        await _ctx.SaveChangesAsync();
        return product.Id;  // SaveChanges 後 EF 自動填回 Id
    }

    public async Task<bool> UpdateAsync(Product product)
    {
        var existing = await _ctx.Products.FindAsync(product.Id);
        if (existing == null) return false;

        existing.Name = product.Name;
        existing.Price = product.Price;
        existing.Stock = product.Stock;
        existing.Category = product.Category;

        await _ctx.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var target = await _ctx.Products.FindAsync(id);
        if (target == null) return false;

        _ctx.Products.Remove(target);
        await _ctx.SaveChangesAsync();
        return true;
    }
}