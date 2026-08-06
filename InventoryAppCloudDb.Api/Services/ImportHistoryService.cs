// ImportHistoryService.cs
using InventoryAppCloudDb.Api.DTOs;
using InventoryAppCloudDb.Api.Models;
using InventoryAppCloudDb.Api.Repositories;

namespace InventoryAppCloudDb.Api.Services;

public class ImportHistoryService : IImportHistoryService
{
    private readonly IImportHistoryRepository _repo;

    public ImportHistoryService(IImportHistoryRepository repo) => _repo = repo;

    public async Task<ServiceResult<ImportCheckResultDto>> CheckAsync(ImportCheckRequestDto dto)
    {
        var existing = await _repo.FindAsync(dto.FileHash, dto.SheetType);

        var result = new ImportCheckResultDto
        {
            AlreadyImported = existing != null,
            ImportedAt = existing?.ImportedAt,
            ImportedBy = existing?.ImportedBy
        };

        return ServiceResult<ImportCheckResultDto>.Ok(result);
    }

    public async Task<ServiceResult> RecordAsync(ImportRecordDto dto, string importedBy)
    {
        await _repo.AddAsync(new ImportHistory
        {
            FileHash = dto.FileHash,
            FileName = dto.FileName,
            SheetType = dto.SheetType,
            ImportedBy = importedBy,
            ImportedAt = DateTime.UtcNow,
        });

        return ServiceResult.Ok();
    }
}