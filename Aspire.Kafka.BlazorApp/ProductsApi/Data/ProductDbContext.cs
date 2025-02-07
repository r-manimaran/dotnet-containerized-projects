using Microsoft.EntityFrameworkCore;
using SharedLib.Models;

namespace ProductsApi.Data;

public class ProductDbContext : DbContext
{
    public ProductDbContext(DbContextOptions<ProductDbContext> options):base(options)
    {
        Database.EnsureCreated();
    }
    public DbSet<Product> Products { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>().HasData(
            new Product { Id = 1, Name = "Shirt", Quantity = 20, Price = 50 },
            new Product { Id = 2, Name = "Mouse", Quantity = 50, Price = 10 },
            new Product { Id = 3, Name = "Milk", Quantity = 30, Price = 5 },
            new Product { Id = 4, Name = "Keyboard", Quantity = 10, Price = 40 });

        base.OnModelCreating(modelBuilder);
    }
}
