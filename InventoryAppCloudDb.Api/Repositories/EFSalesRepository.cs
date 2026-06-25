// Repositories/EFSalesRepository.cs
using InventoryAppCloudDb.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace InventoryAppCloudDb.Api.Repositories;

public class EFSalesRepository : ISalesRepository
{
    private readonly AppDbContext _ctx;

    public EFSalesRepository(AppDbContext ctx) => _ctx = ctx;

    public async Task<List<SalesOrder>> GetAllAsync()
        => await _ctx.SalesOrders
            .Include(o => o.Details).ThenInclude(d => d.Product)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();

    public async Task<SalesOrder?> GetByIdAsync(int id)
        => await _ctx.SalesOrders
            .Include(o => o.Details).ThenInclude(d => d.Product)
            .FirstOrDefaultAsync(o => o.Id == id);

    public async Task<int> InsertAsync(SalesOrder order)
    {
        _ctx.SalesOrders.Add(order);
        await _ctx.SaveChangesAsync();
        return order.Id;
    }

    public async Task<bool> UpdateAsync(SalesOrder order)
    {
        await _ctx.SaveChangesAsync();
        return true;
    }
}