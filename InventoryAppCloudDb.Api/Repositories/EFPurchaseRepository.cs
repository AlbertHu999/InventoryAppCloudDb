// Repositories/EFPurchaseRepository.cs
using InventoryAppCloudDb.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace InventoryAppCloudDb.Api.Repositories;

public class EFPurchaseRepository : IPurchaseRepository
{
    private readonly AppDbContext _ctx;

    public EFPurchaseRepository(AppDbContext ctx) => _ctx = ctx;

    public async Task<List<PurchaseOrder>> GetAllAsync()
        => await _ctx.PurchaseOrders
            .Include(o => o.Details).ThenInclude(d => d.Product)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();

    public async Task<PurchaseOrder?> GetByIdAsync(int id)
        => await _ctx.PurchaseOrders
            .Include(o => o.Details).ThenInclude(d => d.Product)
            .FirstOrDefaultAsync(o => o.Id == id);

    // order.Details 已經組好，EF Core 會自動 cascade insert 明細
    public async Task<int> InsertAsync(PurchaseOrder order)
    {
        _ctx.PurchaseOrders.Add(order);
        await _ctx.SaveChangesAsync();
        return order.Id;
    }

    // 用於之後的作廢（Day 43-44）：修改 Status/VoidedAt 等欄位後存檔
    public async Task<bool> UpdateAsync(PurchaseOrder order)
    {
        await _ctx.SaveChangesAsync();
        return true;
    }
}