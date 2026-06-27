using InventoryAppCloudDb.Api.DTOs;
using InventoryAppCloudDb.Api.Models;
using InventoryAppCloudDb.Api.Repositories;
using Microsoft.EntityFrameworkCore;

namespace InventoryAppCloudDb.Api.Services;
// Services/ProductService.cs
public class ProductService : IProductService
{
    private readonly IProductRepository _repo;
    private readonly AppDbContext _ctx;   // ← 用於刪除前的引用檢查

    // Repository 由 DI 注入
    public ProductService(IProductRepository repo, AppDbContext ctx)
    {
        _repo = repo;
        _ctx = ctx;
    }

    // ── 查詢全部 ──────────────────────────────────────
    public async Task<ServiceResult<List<ProductDto>>> GetAllAsync()
    {
        var products = await _repo.GetAllAsync();

        // Entity → DTO 轉換（不直接把 Entity 往外丟）
        var dtos = products.Select(p => ToDto(p)).ToList();

        return ServiceResult<List<ProductDto>>.Ok(dtos);
    }

    // ── 查詢單筆 ──────────────────────────────────────
    public async Task<ServiceResult<ProductDto>> GetByIdAsync(int id)
    {
        var product = await _repo.GetByIdAsync(id);

        if (product == null)
            return ServiceResult<ProductDto>.Fail($"找不到 Id={id} 的商品");

        return ServiceResult<ProductDto>.Ok(ToDto(product));
    }
    // ── 依分類查詢 ──────────────────────────────────────
    public async Task<ServiceResult<List<ProductDto>>> GetByCategoryAsync(string category)
    {
        var products = await _repo.GetByCategoryAsync(category);
        var dtos = products.Select(p => ToDto(p)).ToList();
        return ServiceResult<List<ProductDto>>.Ok(dtos);
    }


    // ── 新增 ──────────────────────────────────────────
    public async Task<ServiceResult<ProductDto>> CreateAsync(CreateProductDto dto)
    {
        // ✅ 商業規則驗證（這裡才是正確的位置）
        if (dto.Price < 0)
            return ServiceResult<ProductDto>.Fail("售價不能為負數");

        if (string.IsNullOrWhiteSpace(dto.Name))
            return ServiceResult<ProductDto>.Fail("商品名稱不能空白");

        // DTO → Entity 轉換
        var product = new Product
        {
            Name = dto.Name.Trim(),
            Price = dto.Price,
            Stock = dto.Stock,
            Category = dto.Category.Trim(),
            CreatedAt = DateTime.UtcNow,
        };

        var newId = await _repo.InsertAsync(product);
        product.Id = newId;

        return ServiceResult<ProductDto>.Ok(ToDto(product));
    }

    // ── 修改 ──────────────────────────────────────────
    public async Task<ServiceResult<ProductDto>> UpdateAsync(int id, UpdateProductDto dto)
    {
        // 先確認資料存在
        var existing = await _repo.GetByIdAsync(id);
        if (existing == null)
            return ServiceResult<ProductDto>.Fail($"找不到 Id={id} 的商品");

        // 商業規則驗證
        if (dto.Price < 0)
            return ServiceResult<ProductDto>.Fail("售價不能為負數");

        if (string.IsNullOrWhiteSpace(dto.Name))
            return ServiceResult<ProductDto>.Fail("商品名稱不能空白");

        // 更新 Entity（不含 Stock，庫存只能透過進銷貨異動）
        existing.Name = dto.Name.Trim();
        existing.Price = dto.Price;
        existing.Category = dto.Category.Trim();
        await _repo.UpdateAsync(existing);

        return ServiceResult<ProductDto>.Ok(ToDto(existing));
    }

    // ── 刪除 ──────────────────────────────────────────
    // ── 刪除（加保護：已被引用則拒絕真刪，引導改用停用）──
    public async Task<ServiceResult> DeleteAsync(int id)
    {
        var existing = await _repo.GetByIdAsync(id);
        if (existing == null)
            return ServiceResult.Fail($"找不到 Id={id} 的商品");

        // 檢查是否被任何單據引用過
        var hasBeenUsed =
            await _ctx.PurchaseOrderDetails.AnyAsync(d => d.ProductId == id)
         || await _ctx.SalesOrderDetails.AnyAsync(d => d.ProductId == id)
         || await _ctx.InventoryLedgers.AnyAsync(l => l.ProductId == id);

        if (hasBeenUsed)
            return ServiceResult.Fail(
                "此商品已有進貨/銷貨記錄，無法刪除，請改用「停用」功能保留歷史資料");

        await _repo.DeleteAsync(id);
        return ServiceResult.Ok();
    }
    // ── 私有轉換方法：Entity → DTO ────────────────────
    // 集中在這一個地方，之後欄位有變動只改這裡
    private static ProductDto ToDto(Product p) => new()
    {
        Id = p.Id,
        Name = p.Name,
        Price = p.Price,
        Stock = p.Stock,
        Category = p.Category,
    };

    // ── Phase 5.5 Day43-44：停用商品（不刪除，保留歷史單據可追溯）──
   public async Task<ServiceResult> DeactivateAsync(int id)
    {
        var product = await _repo.GetByIdAsync(id);
        if (product == null)
            return ServiceResult.Fail($"找不到 Id={id} 的商品");

        await _repo.UpdateActiveStatusAsync(id, false);   // ← 改用專屬方法
        return ServiceResult.Ok();
    }

    // ── Phase 5.5 Day43-44：重新啟用商品 ──
    public async Task<ServiceResult> ActivateAsync(int id)
    {
        var product = await _repo.GetByIdAsync(id);
        if (product == null)
            return ServiceResult.Fail($"找不到 Id={id} 的商品");

        await _repo.UpdateActiveStatusAsync(id, true);    // ← 改用專屬方法
        return ServiceResult.Ok();
    }
}