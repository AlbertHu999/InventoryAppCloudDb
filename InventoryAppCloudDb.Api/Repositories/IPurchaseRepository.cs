// Repositories/IPurchaseRepository.cs
using InventoryAppCloudDb.Api.Models;

namespace InventoryAppCloudDb.Api.Repositories;

public interface IPurchaseRepository
{
    Task<List<PurchaseOrder>> GetAllAsync();
    Task<PurchaseOrder?> GetByIdAsync(int id);
    Task<int> InsertAsync(PurchaseOrder order);
    Task<bool> UpdateAsync(PurchaseOrder order);
}