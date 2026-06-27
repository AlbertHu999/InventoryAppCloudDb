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

    // ── 一般編輯：只更新純資料欄位（不碰 Stock / IsActive）──
    public async Task<bool> UpdateAsync(Product product)
    {
        var existing = await _ctx.Products.FindAsync(product.Id);
        if (existing == null) return false;

        // 只更新「純資料」欄位
        existing.Name = product.Name;
        existing.Price = product.Price;
        existing.Category = product.Category;
        existing.UpdatedAt = DateTime.UtcNow;

        // ⚠️ 不碰 IsActive：只能透過 Deactivate / Activate 變更
        // ⚠️ 不碰 Stock：只能透過進貨/銷貨/作廢（UpdateStockAsync + 流水帳）變更
        //    一般編輯改庫存會繞過流水帳，破壞稽核完整性

        await _ctx.SaveChangesAsync();
        return true;
    }

    // ── Phase 5.5：只更新庫存（進銷貨/作廢異動專用，搭配流水帳）──
    public async Task UpdateStockAsync(int productId, int newStock)
    {
        var product = await _ctx.Products.FindAsync(productId);
        if (product != null)
        {
            product.Stock = newStock;
            await _ctx.SaveChangesAsync();
        }
    }
    
    // ── Phase 5.5：只更新啟用狀態（停用/啟用專用）──
    public async Task UpdateActiveStatusAsync(int productId, bool isActive)
    {
        var product = await _ctx.Products.FindAsync(productId);
        if (product != null)
        {
            product.IsActive = isActive;
            product.UpdatedAt = DateTime.UtcNow;
            await _ctx.SaveChangesAsync();
        }
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