using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryAppCloudDb.Api.Models;

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
}