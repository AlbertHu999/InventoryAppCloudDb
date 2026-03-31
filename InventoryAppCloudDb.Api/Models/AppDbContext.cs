using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace InventoryAppCloudDb.Api.Models;

public class AppDbContext : DbContext
{
    public DbSet<Product> Products { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>()
            .Property(p => p.Price)
            .HasPrecision(10, 2);

        modelBuilder.Entity<Product>()
            .Property(p => p.Id)
            .UseIdentityAlwaysColumn();
    }
}