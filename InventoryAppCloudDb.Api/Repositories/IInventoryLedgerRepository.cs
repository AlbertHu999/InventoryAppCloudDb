// Repositories/IInventoryLedgerRepository.cs
using InventoryAppCloudDb.Api.Models;

namespace InventoryAppCloudDb.Api.Repositories;

public interface IInventoryLedgerRepository
{
    Task AddRangeAsync(List<InventoryLedger> entries);
    Task<List<InventoryLedger>> GetAllAsync();
    Task<List<InventoryLedger>> GetByProductIdAsync(int productId);
}