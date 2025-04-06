using Microsoft.EntityFrameworkCore;
using SalesApi.Models;

namespace SalesApi.Data;
public class SalesDbContext: DbContext
{
    public SalesDbContext(DbContextOptions<SalesDbContext> options):base(options)
    {
        
    }
    public DbSet<Sale> Sales { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Sale>().HasKey(x => x.Id);
    }
}

