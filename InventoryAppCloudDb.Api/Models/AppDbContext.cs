using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace InventoryAppCloudDb.Api.Models;

public class AppDbContext : DbContext
{
    public DbSet<Product> Products { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<UserToken> UserTokens { get; set; }
    public DbSet<PurchaseOrder> PurchaseOrders { get; set; }  
    public DbSet<PurchaseOrderDetail> PurchaseOrderDetails { get; set; }  
    public DbSet<SalesOrder> SalesOrders { get; set; }  
    public DbSet<SalesOrderDetail> SalesOrderDetails { get; set; }



    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // 原本就有的
        modelBuilder.Entity<Product>()
            .Property(p => p.Price)
            .HasPrecision(10, 2);

        modelBuilder.Entity<Product>()
            .Property(p => p.Id)
            .UseIdentityAlwaysColumn();

        // 新增的精度設定
        modelBuilder.Entity<PurchaseOrderDetail>()
            .Property(d => d.UnitPrice)
            .HasPrecision(10, 2);

        modelBuilder.Entity<SalesOrderDetail>()
            .Property(d => d.UnitPrice)
            .HasPrecision(10, 2);

        // 新增的 Id 設定
        modelBuilder.Entity<PurchaseOrder>()
            .Property(p => p.Id)
            .UseIdentityAlwaysColumn();

        modelBuilder.Entity<PurchaseOrderDetail>()
            .Property(p => p.Id)
            .UseIdentityAlwaysColumn();

        modelBuilder.Entity<SalesOrder>()
            .Property(p => p.Id)
            .UseIdentityAlwaysColumn();

        modelBuilder.Entity<SalesOrderDetail>()
            .Property(p => p.Id)
            .UseIdentityAlwaysColumn();
    }
}