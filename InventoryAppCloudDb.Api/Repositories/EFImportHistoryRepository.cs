using InventoryAppCloudDb.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace InventoryAppCloudDb.Api.Repositories;

public class EFImportHistoryRepository : IImportHistoryRepository
{
    private readonly AppDbContext _ctx;

    public EFImportHistoryRepository(AppDbContext ctx) => _ctx = ctx;

    public async Task<ImportHistory?> FindAsync(string fileHash, string sheetType)
        => await _ctx.ImportHistories
            .Where(h => h.FileHash == fileHash && h.SheetType == sheetType)
            .OrderByDescending(h => h.ImportedAt)
            .FirstOrDefaultAsync();

    public async Task AddAsync(ImportHistory history)
    {
        _ctx.ImportHistories.Add(history);
        await _ctx.SaveChangesAsync();
    }
}