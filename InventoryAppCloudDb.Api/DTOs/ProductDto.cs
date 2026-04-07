using System.ComponentModel.DataAnnotations;

namespace InventoryAppCloudDb.Api.DTOs;

// 查詢回傳用（API 回傳給前端的資料格式）
public class ProductDto
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public string Category { get; set; } = "";
}

// 新增時前端傳入的資料（不含 Id，因為 Id 由資料庫產生）
public class CreateProductDto
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = "";

    [Range(0, 9999999)]
    public decimal Price { get; set; }

    [Range(0, int.MaxValue)]
    public int Stock { get; set; }

    [MaxLength(50)]
    public string Category { get; set; } = "";
}

// 修改時前端傳入的資料（不含 Id，Id 從 URL 路徑取得）
public class UpdateProductDto
{
    [Required]
    [MaxLength(100)]
    public string  Name     { get; set; } = "";

    [Range(0, 9999999)]
    public decimal Price    { get; set; }

    [Range(0, int.MaxValue)]
    public int     Stock    { get; set; }

    [MaxLength(50)]
    public string  Category { get; set; } = "";
}