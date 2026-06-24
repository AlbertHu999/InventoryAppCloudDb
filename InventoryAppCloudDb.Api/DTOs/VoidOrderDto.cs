// DTOs/VoidOrderDto.cs
namespace InventoryAppCloudDb.Api.DTOs;

public class VoidOrderDto
{
    public string? Reason { get; set; }   // 作廢原因（前端傳入，可不填）
}