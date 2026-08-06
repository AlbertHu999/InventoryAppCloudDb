namespace InventoryAppCloudDb.Api.DTOs;

public class ImportCheckRequestDto
{
    public string FileHash { get; set; } = "";
    public string SheetType { get; set; } = "";
}

public class ImportCheckResultDto
{
    public bool AlreadyImported { get; set; }
    public DateTime? ImportedAt { get; set; }
    public string? ImportedBy { get; set; }
}

public class ImportRecordDto
{
    public string FileHash { get; set; } = "";
    public string FileName { get; set; } = "";
    public string SheetType { get; set; } = "";
}