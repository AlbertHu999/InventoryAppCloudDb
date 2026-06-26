using Microsoft.EntityFrameworkCore;
using InventoryAppCloudDb.Api.DTOs;
using InventoryAppCloudDb.Api.Models;
using InventoryAppCloudDb.Api.Repositories;

namespace InventoryAppCloudDb.Api.Services;

public class SalesService : ISalesService
{
    private readonly AppDbContext _ctx;
    private readonly ISalesRepository _salesRepo;
    private readonly IInventoryLedgerRepository _ledgerRepo;
    private readonly IProductRepository _productRepo;

    public SalesService(
        AppDbContext ctx,
        ISalesRepository salesRepo,
        IInventoryLedgerRepository ledgerRepo,
        IProductRepository productRepo)
    {
        _ctx = ctx;
        _salesRepo = salesRepo;
        _ledgerRepo = ledgerRepo;
        _productRepo = productRepo;
    }

    public async Task<ServiceResult<List<SalesOrderDto>>> GetAllAsync()
    {
        var orders = await _salesRepo.GetAllAsync();
        return ServiceResult<List<SalesOrderDto>>.Ok(
            orders.Select(ToDto).ToList());
    }

    public async Task<ServiceResult<SalesOrderDto>> GetByIdAsync(int id)
    {
        var order = await _salesRepo.GetByIdAsync(id);
        return order == null
            ? ServiceResult<SalesOrderDto>.Fail($"找不到 Id={id} 的銷貨單")
            : ServiceResult<SalesOrderDto>.Ok(ToDto(order));
    }

    public async Task<ServiceResult<SalesOrderDto>> CreateAsync(
        CreateSalesOrderDto dto, string createdBy)
    {
        if (!dto.Details.Any())
            return ServiceResult<SalesOrderDto>.Fail("銷貨單至少需要一筆明細");

        foreach (var d in dto.Details)
        {
            if (d.Quantity <= 0)
                return ServiceResult<SalesOrderDto>.Fail("銷貨數量必須大於 0");
            if (d.UnitPrice < 0)
                return ServiceResult<SalesOrderDto>.Fail("銷貨單價不能為負數");
        }

        // ✅✅✅ 核心修正：依 ProductId 彙總整張單的需求量，不是逐筆檢查 ✅✅✅
        var requiredByProduct = dto.Details
            .GroupBy(d => d.ProductId)
            .Select(g => new { ProductId = g.Key, TotalQuantity = g.Sum(x => x.Quantity) })
            .ToList();

        using var tx = await _ctx.Database.BeginTransactionAsync();
        try
        {
            // ── 驗證階段：先把彙總後的需求量全部驗證過，一張都不放過才繼續 ──
            foreach (var req in requiredByProduct)
            {
                var product = await _productRepo.GetByIdAsync(req.ProductId);
                if (product == null)
                {
                    await tx.RollbackAsync();
                    return ServiceResult<SalesOrderDto>.Fail(
                        $"找不到 ProductId={req.ProductId} 的商品");
                }
                if (!product.IsActive)
                {
                    await tx.RollbackAsync();
                    return ServiceResult<SalesOrderDto>.Fail(
                        $"「{product.Name}」已停用，無法銷貨");
                }
                if (product.Stock < req.TotalQuantity)
                {
                    await tx.RollbackAsync();
                    return ServiceResult<SalesOrderDto>.Fail(
                        $"「{product.Name}」庫存不足（現有 {product.Stock}，整張單共需 {req.TotalQuantity}）");
                }
            }

            // ── 驗證全部通過，才開始真正建立單據與異動庫存 ──
            var order = new SalesOrder
            {
                Customer = dto.Customer.Trim(),
                Note = dto.Note.Trim(),
                CreatedBy = createdBy,
                OrderDate = DateTime.UtcNow,
                Status = "Posted",
            };
            foreach (var d in dto.Details)
            {
                order.Details.Add(new SalesOrderDetail
                {
                    ProductId = d.ProductId,
                    Quantity = d.Quantity,
                    UnitPrice = d.UnitPrice,
                });
            }

            var newId = await _salesRepo.InsertAsync(order);

            var ledgers = new List<InventoryLedger>();
            foreach (var detail in order.Details)
            {
                var product = await _productRepo.GetByIdAsync(detail.ProductId);
                await _productRepo.UpdateStockAsync(
                    detail.ProductId, product!.Stock - detail.Quantity);

                ledgers.Add(new InventoryLedger
                {
                    ProductId = detail.ProductId,
                    SourceType = "Sales",
                    SourceOrderId = newId,
                    SourceDetailId = detail.Id,
                    Direction = "Out",    // ⚠️ 明確賦值，絕不可漏！
                    Quantity = detail.Quantity,
                    UnitPrice = detail.UnitPrice,
                    CreatedBy = createdBy,
                });
            }
            await _ledgerRepo.AddRangeAsync(ledgers);

            await tx.CommitAsync();
            return await GetByIdAsync(newId);
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
        Status = o.Status,
        Details = o.Details.Select(d => new SalesDetailResponseDto
        {
            ProductId = d.ProductId,
            ProductName = d.Product?.Name ?? "",
            Quantity = d.Quantity,
            UnitPrice = d.UnitPrice,
        }).ToList(),
    };
}