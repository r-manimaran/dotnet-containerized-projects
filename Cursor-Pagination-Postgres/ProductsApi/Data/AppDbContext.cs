

using Microsoft.EntityFrameworkCore;
using ProductsApi.Models;

namespace ProductsApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) :base(options)
    {
        
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        // Apply snake_case naming convention
        //configurationBuilder.Conventions.UseSnakeCaseNamingConvention();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired();
            entity.Property(e => e.CreatedDate).HasColumnType("timestamp without time zone");
            entity.Property(e => e.ManufactoringDate).HasColumnType("timestamp without time zone");
        });
    }

    public DbSet<Product> Products { get; set; }
}
