using Aspire.Database.TestContainers.Data.Configurations;
using Aspire.Database.TestContainers.Models;
using Microsoft.EntityFrameworkCore;

namespace Aspire.Database.TestContainers.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options): base(options)
    {
        
    }
    public DbSet<Product> Products { get; set; }
    public DbSet<Todo> Todos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new ProductConfiguration());
    }
}
