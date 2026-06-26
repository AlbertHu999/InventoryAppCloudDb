using Microsoft.EntityFrameworkCore;
using InventoryAppCloudDb.Api.DTOs;
using InventoryAppCloudDb.Api.Models;
using InventoryAppCloudDb.Api.Repositories;

namespace InventoryAppCloudDb.Api.Services;

public class PurchaseService : IPurchaseService
{
    private readonly AppDbContext _ctx;          // 只用來控制 Transaction
    private readonly IPurchaseRepository _purchaseRepo;
    private readonly IInventoryLedgerRepository _ledgerRepo;
    private readonly IProductRepository _productRepo;

    public PurchaseService(
        AppDbContext ctx,
        IPurchaseRepository purchaseRepo,
        IInventoryLedgerRepository ledgerRepo,
        IProductRepository productRepo)
    {
        _ctx = ctx;
        _purchaseRepo = purchaseRepo;
        _ledgerRepo = ledgerRepo;
        _productRepo = productRepo;
    }

    public async Task<ServiceResult<List<PurchaseOrderDto>>> GetAllAsync()
    {
        var orders = await _purchaseRepo.GetAllAsync();
        return ServiceResult<List<PurchaseOrderDto>>.Ok(
            orders.Select(ToDto).ToList());
    }

    public async Task<ServiceResult<PurchaseOrderDto>> GetByIdAsync(int id)
    {
        var order = await _purchaseRepo.GetByIdAsync(id);
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
                Status = "Posted",   // 雖然有 DEFAULT，這裡明確寫出更清楚
            };

            // ── 驗證階段：先把每筆明細的商品都檢查過 ──
            foreach (var d in dto.Details)
            {
                var product = await _productRepo.GetByIdAsync(d.ProductId);
                if (product == null)
                {
                    await tx.RollbackAsync();
                    return ServiceResult<PurchaseOrderDto>.Fail(
                        $"找不到 ProductId={d.ProductId} 的商品");
                }
                if (!product.IsActive)
                {
                    await tx.RollbackAsync();
                    return ServiceResult<PurchaseOrderDto>.Fail(
                        $"「{product.Name}」已停用，無法進貨");
                }

                order.Details.Add(new PurchaseOrderDetail
                {
                    ProductId = d.ProductId,
                    Quantity = d.Quantity,
                    UnitPrice = d.UnitPrice,
                });
            }

            // ── 寫入主檔 + 明細（一次 Insert，EF Core 自動 cascade）──
            var newId = await _purchaseRepo.InsertAsync(order);

            // ── 逐筆更新庫存 + 寫入 InventoryLedger ──
            var ledgers = new List<InventoryLedger>();
            foreach (var detail in order.Details)
            {
                var product = await _productRepo.GetByIdAsync(detail.ProductId);
                await _productRepo.UpdateStockAsync(
                    detail.ProductId, product!.Stock + detail.Quantity);

                ledgers.Add(new InventoryLedger
                {
                    ProductId = detail.ProductId,
                    SourceType = "Purchase",
                    SourceOrderId = newId,
                    SourceDetailId = detail.Id,
                    Direction = "In",     // ⚠️ 明確賦值，絕不可漏！沒有 DEFAULT
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
        Status = o.Status,
        Details = o.Details.Select(d => new PurchaseDetailResponseDto
        {
            ProductId = d.ProductId,
            ProductName = d.Product?.Name ?? "",
            Quantity = d.Quantity,
            UnitPrice = d.UnitPrice,
        }).ToList(),
    };
}