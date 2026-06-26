// Repositories/EFInventoryLedgerRepository.cs
using InventoryAppCloudDb.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace InventoryAppCloudDb.Api.Repositories;

public class EFInventoryLedgerRepository : IInventoryLedgerRepository
{
    private readonly AppDbContext _ctx;

    public EFInventoryLedgerRepository(AppDbContext ctx) => _ctx = ctx;

    // 一次寫入多筆流水帳（一張單可能有多筆明細）
    public async Task AddRangeAsync(List<InventoryLedger> entries)
    {
        _ctx.InventoryLedgers.AddRange(entries);
        await _ctx.SaveChangesAsync();
    }

    public async Task<List<InventoryLedger>> GetAllAsync()
        => await _ctx.InventoryLedgers
            .Include(l => l.Product)
            .OrderByDescending(l => l.CreatedAt)
            .ToListAsync();

    public async Task<List<InventoryLedger>> GetByProductIdAsync(int productId)
        => await _ctx.InventoryLedgers
            .Where(l => l.ProductId == productId)
            .OrderByDescending(l => l.CreatedAt)
            .ToListAsync();
}