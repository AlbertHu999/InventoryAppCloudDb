// IImportHistoryService.cs
using InventoryAppCloudDb.Api.DTOs;
using InventoryAppCloudDb.Api.Models;

namespace InventoryAppCloudDb.Api.Services;

public interface IImportHistoryService
{
    Task<ServiceResult<ImportCheckResultDto>> CheckAsync(ImportCheckRequestDto dto);
    Task<ServiceResult> RecordAsync(ImportRecordDto dto, string importedBy);
}