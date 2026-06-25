// Repositories/ISalesRepository.cs
using InventoryAppCloudDb.Api.Models;

namespace InventoryAppCloudDb.Api.Repositories;

public interface ISalesRepository
{
    Task<List<SalesOrder>> GetAllAsync();
    Task<SalesOrder?> GetByIdAsync(int id);
    Task<int> InsertAsync(SalesOrder order);
    Task<bool> UpdateAsync(SalesOrder order);
}