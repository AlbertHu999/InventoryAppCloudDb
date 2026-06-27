// Services/InventoryService.cs
using InventoryAppCloudDb.Api.DTOs;
using InventoryAppCloudDb.Api.Models;
using InventoryAppCloudDb.Api.Repositories;

namespace InventoryAppCloudDb.Api.Services;

public class InventoryService : IInventoryService
{
    private readonly IInventoryLedgerRepository _ledgerRepo;
    private readonly IProductRepository _productRepo;

    public InventoryService(
        IInventoryLedgerRepository ledgerRepo,
        IProductRepository productRepo)
    {
        _ledgerRepo = ledgerRepo;
        _productRepo = productRepo;
    }

    public async Task<ServiceResult<List<InventoryLedgerDto>>> GetAllAsync()
    {
        var ledgers = await _ledgerRepo.GetAllAsync();
        return ServiceResult<List<InventoryLedgerDto>>.Ok(
            ledgers.Select(ToDto).ToList());
    }

    public async Task<ServiceResult<List<InventoryLedgerDto>>> GetByProductIdAsync(int productId)
    {
        var ledgers = await _ledgerRepo.GetByProductIdAsync(productId);
        return ServiceResult<List<InventoryLedgerDto>>.Ok(
            ledgers.Select(ToDto).ToList());
    }

    private static InventoryLedgerDto ToDto(InventoryLedger l) => new()
    {
        Id = l.Id,
        ProductId = l.ProductId,
        ProductName = l.Product?.Name ?? "",
        SourceType = l.SourceType,
        SourceOrderId = l.SourceOrderId,
        Direction = l.Direction,
        Quantity = l.Quantity,
        UnitPrice = l.UnitPrice,
        CreatedAt = l.CreatedAt,
        CreatedBy = l.CreatedBy,
        Remark = l.Remark,
    };
}