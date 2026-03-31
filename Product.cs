using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryAppCloudDb;

[Table("products")]
public class Product
{
    [Column("id")]
    public int Id { get; set; }

    [Column("name")]
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = "";

    [Column("price")]
    [Range(0, 9999999)]
    public double Price { get; set; }

    [Column("stock")]
    public int Stock { get; set; }

    [Column("category")]
    [MaxLength(50)]
    public string Category { get; set; } = "";

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public bool IsLowStock => Stock < 10;

    // 保留原本的建構子
    public Product() { }

    public Product(int id, string name, double price, int stock, string category)
    {
        Id = id;
        Name = name;
        Price = price;
        Stock = stock;
        Category = category;
    }
}