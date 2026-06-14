using Microsoft.EntityFrameworkCore;
using InventoryAppCloudDb.Api.DTOs;
using InventoryAppCloudDb.Api.Models;

namespace InventoryAppCloudDb.Api.Services;

public class PurchaseService : IPurchaseService
{
    private readonly AppDbContext _ctx;

    public PurchaseService(AppDbContext ctx) => _ctx = ctx;

    public async Task<ServiceResult<List<PurchaseOrderDto>>> GetAllAsync()
    {
        var orders = await _ctx.PurchaseOrders
            .Include(o => o.Details).ThenInclude(d => d.Product)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();

        return ServiceResult<List<PurchaseOrderDto>>.Ok(
            orders.Select(ToDto).ToList());
    }

    public async Task<ServiceResult<PurchaseOrderDto>> GetByIdAsync(int id)
    {
        var order = await _ctx.PurchaseOrders
            .Include(o => o.Details).ThenInclude(d => d.Product)
            .FirstOrDefaultAsync(o => o.Id == id);

        return order == null
            ? ServiceResult<PurchaseOrderDto>.Fail($"找不到 Id={id} 的進貨單")
            : ServiceResult<PurchaseOrderDto>.Ok(ToDto(order));
    }

    public async Task<ServiceResult<PurchaseOrderDto>> CreateAsync(
        CreatePurchaseOrderDto dto, string createdBy)
    {
        if (!dto.Details.Any())
            return ServiceResult<PurchaseOrderDto>.Fail("進貨單至少需要一筆明細");

        foreach (var d in dto.Details)
        {
            if (d.Quantity <= 0)
                return ServiceResult<PurchaseOrderDto>.Fail("進貨數量必須大於 0");
            if (d.UnitPrice < 0)
                return ServiceResult<PurchaseOrderDto>.Fail("進貨單價不能為負數");
        }

        using var tx = await _ctx.Database.BeginTransactionAsync();
        try
        {
            var order = new PurchaseOrder
            {
                Supplier = dto.Supplier.Trim(),
                Note = dto.Note.Trim(),
                CreatedBy = createdBy,
                OrderDate = DateTime.UtcNow,
            };
            _ctx.PurchaseOrders.Add(order);
            await _ctx.SaveChangesAsync();  // 先存，取得 order.Id

            foreach (var d in dto.Details)
            {
                var product = await _ctx.Products.FindAsync(d.ProductId);
                if (product == null)
                {
                    await tx.RollbackAsync();
                    return ServiceResult<PurchaseOrderDto>.Fail(
                        $"找不到 ProductId={d.ProductId} 的商品");
                }

                _ctx.PurchaseOrderDetails.Add(new PurchaseOrderDetail
                {
                    PurchaseOrderId = order.Id,
                    ProductId = d.ProductId,
                    Quantity = d.Quantity,
                    UnitPrice = d.UnitPrice,
                });

                // 進貨 → 庫存增加
                product.Stock += d.Quantity;
            }

            await _ctx.SaveChangesAsync();
            await tx.CommitAsync();

            return await GetByIdAsync(order.Id);
        }
        catch (Exception ex)
        {
            await tx.RollbackAsync();
            return ServiceResult<PurchaseOrderDto>.Fail($"進貨失敗：{ex.Message}");
        }
    }

    private static PurchaseOrderDto ToDto(PurchaseOrder o) => new()
    {
        Id = o.Id,
        OrderDate = o.OrderDate,
        Supplier = o.Supplier,
        Note = o.Note,
        CreatedBy = o.CreatedBy,
        Details = o.Details.Select(d => new PurchaseDetailResponseDto
        {
            ProductId = d.ProductId,
            ProductName = d.Product?.Name ?? "",
            Quantity = d.Quantity,
            UnitPrice = d.UnitPrice,
        }).ToList(),
    };
}