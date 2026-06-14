using Microsoft.EntityFrameworkCore;
using InventoryAppCloudDb.Api.DTOs;
using InventoryAppCloudDb.Api.Models;

namespace InventoryAppCloudDb.Api.Services;

public class SalesService : ISalesService
{
    private readonly AppDbContext _ctx;

    public SalesService(AppDbContext ctx) => _ctx = ctx;

    public async Task<ServiceResult<List<SalesOrderDto>>> GetAllAsync()
    {
        var orders = await _ctx.SalesOrders
            .Include(o => o.Details).ThenInclude(d => d.Product)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();

        return ServiceResult<List<SalesOrderDto>>.Ok(
            orders.Select(ToDto).ToList());
    }

    public async Task<ServiceResult<SalesOrderDto>> GetByIdAsync(int id)
    {
        var order = await _ctx.SalesOrders
            .Include(o => o.Details).ThenInclude(d => d.Product)
            .FirstOrDefaultAsync(o => o.Id == id);

        return order == null
            ? ServiceResult<SalesOrderDto>.Fail($"找不到 Id={id} 的銷貨單")
            : ServiceResult<SalesOrderDto>.Ok(ToDto(order));
    }

    public async Task<ServiceResult<SalesOrderDto>> CreateAsync(
        CreateSalesOrderDto dto, string createdBy)
    {
        if (!dto.Details.Any())
            return ServiceResult<SalesOrderDto>.Fail("銷貨單至少需要一筆明細");

        using var tx = await _ctx.Database.BeginTransactionAsync();
        try
        {
            var order = new SalesOrder
            {
                Customer = dto.Customer.Trim(),
                Note = dto.Note.Trim(),
                CreatedBy = createdBy,
                OrderDate = DateTime.UtcNow,
            };
            _ctx.SalesOrders.Add(order);
            await _ctx.SaveChangesAsync();

            foreach (var d in dto.Details)
            {
                var product = await _ctx.Products.FindAsync(d.ProductId);
                if (product == null)
                {
                    await tx.RollbackAsync();
                    return ServiceResult<SalesOrderDto>.Fail(
                        $"找不到 ProductId={d.ProductId} 的商品");
                }

                // 庫存不足擋住
                if (product.Stock < d.Quantity)
                {
                    await tx.RollbackAsync();
                    return ServiceResult<SalesOrderDto>.Fail(
                        $"「{product.Name}」庫存不足（現有 {product.Stock}，要出 {d.Quantity}）");
                }

                _ctx.SalesOrderDetails.Add(new SalesOrderDetail
                {
                    SalesOrderId = order.Id,
                    ProductId = d.ProductId,
                    Quantity = d.Quantity,
                    UnitPrice = d.UnitPrice,
                });

                // 銷貨 → 庫存扣除
                product.Stock -= d.Quantity;
            }

            await _ctx.SaveChangesAsync();
            await tx.CommitAsync();

            return await GetByIdAsync(order.Id);
        }
        catch (Exception ex)
        {
            await tx.RollbackAsync();
            return ServiceResult<SalesOrderDto>.Fail($"銷貨失敗：{ex.Message}");
        }
    }

    private static SalesOrderDto ToDto(SalesOrder o) => new()
    {
        Id = o.Id,
        OrderDate = o.OrderDate,
        Customer = o.Customer,
        Note = o.Note,
        CreatedBy = o.CreatedBy,
        Details = o.Details.Select(d => new SalesDetailResponseDto
        {
            ProductId = d.ProductId,
            ProductName = d.Product?.Name ?? "",
            Quantity = d.Quantity,
            UnitPrice = d.UnitPrice,
        }).ToList(),
    };
}