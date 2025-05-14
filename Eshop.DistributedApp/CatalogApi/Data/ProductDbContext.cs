using CatalogApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CatalogApi.Data;

public class ProductDbContext : DbContext
{
    public ProductDbContext(DbContextOptions<ProductDbContext> options):base(options)
    {
        
    }
    public DbSet<Product> Products => Set<Product>();
}
