// DTOs/ImportCheckResultDto.cs
namespace InventoryAppCloudDb.DTOs;

public class ImportCheckResultDto
{
    public bool AlreadyImported { get; set; }
    public DateTime? ImportedAt { get; set; }
    public string? ImportedBy { get; set; }
}