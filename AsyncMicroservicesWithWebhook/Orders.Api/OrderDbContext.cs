using Microsoft.EntityFrameworkCore;
using Shared.Contracts.Models;

namespace Orders.Api;

public class OrderDbContext : DbContext
{
    public OrderDbContext(DbContextOptions<OrderDbContext> options) :base(options)
    {
        
    }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderProduct> OrderProducts { get; set; }

    //Copy of Product will be placed in this datbase. Incase Products service is failed to communicate we can use this table
    public DbSet<Product> Products { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // 1 order will have multiple Products
        modelBuilder.Entity<Order>()
                    .HasMany<OrderProduct>()
                    .WithOne(o => o.Order)
                    .HasForeignKey(o => o.OrderId);

        modelBuilder.Entity<OrderProduct>()
                   .HasOne(op => op.Order)
                   .WithMany(o => o.Products)
                   .HasForeignKey(op => op.OrderId);

        modelBuilder.Entity<OrderProduct>()
                    .HasOne(op => op.Product)
                    .WithMany()
                    .HasForeignKey(op => op.ProductId);

    }
}
