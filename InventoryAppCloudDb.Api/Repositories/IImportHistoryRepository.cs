using InventoryAppCloudDb.Api.Models;

namespace InventoryAppCloudDb.Api.Repositories;

public interface IImportHistoryRepository
{
    Task<ImportHistory?> FindAsync(string fileHash, string sheetType);
    Task AddAsync(ImportHistory history);
}