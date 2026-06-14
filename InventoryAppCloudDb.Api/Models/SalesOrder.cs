using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryAppCloudDb.Api.Models;

[Table("sales_orders")]
public class SalesOrder
{
    [Column("id")]
    public int Id { get; set; }

    [Column("order_date")]
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;

    [Column("customer")]
    [MaxLength(100)]
    public string Customer { get; set; } = "";

    [Column("note")]
    [MaxLength(200)]
    public string Note { get; set; } = "";

    [Column("created_by")]
    [MaxLength(50)]
    public string CreatedBy { get; set; } = "";

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public List<SalesOrderDetail> Details { get; set; } = [];
}