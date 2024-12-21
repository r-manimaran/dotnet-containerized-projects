using Microsoft.EntityFrameworkCore;
using Shared.Contracts.Models;

namespace Products.Api;

public class ProductDbContext :DbContext
{
    public ProductDbContext(DbContextOptions<ProductDbContext> options):base(options)
    {
        
    }
    public DbSet<Product> Products { get; set; }
}
