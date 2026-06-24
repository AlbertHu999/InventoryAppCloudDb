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
    public DbSet<InventoryLedger> InventoryLedgers { get; set; }   // ← Phase 5.5 新增


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

        // ── Phase 5.5 新增：InventoryLedger 設定 ──
        modelBuilder.Entity<InventoryLedger>()
            .Property(l => l.UnitPrice)
            .HasPrecision(10, 2);

        modelBuilder.Entity<InventoryLedger>()
            .Property(l => l.Id)
            .UseIdentityAlwaysColumn();

        modelBuilder.Entity<InventoryLedger>()
                    .HasIndex(l => l.ProductId);   // 查單一商品流水帳會用到，建索引加速

        // ── Phase 5.5 新增：資料庫 DEFAULT 值設定 ──
        // 目的：讓 Migration 對「既有資料列」補欄位時，使用正確預設值
        // （C# 屬性的 = true 只管 new 物件，管不到資料庫，必須在這裡明確宣告）
        modelBuilder.Entity<Product>()
            .Property(p => p.IsActive)
            .HasDefaultValue(true);

        modelBuilder.Entity<User>()
            .Property(u => u.IsActive)
            .HasDefaultValue(true);

        modelBuilder.Entity<PurchaseOrder>()
            .Property(o => o.Status)
            .HasDefaultValue("Posted");

        modelBuilder.Entity<SalesOrder>()
            .Property(o => o.Status)
            .HasDefaultValue("Posted");
    }
}