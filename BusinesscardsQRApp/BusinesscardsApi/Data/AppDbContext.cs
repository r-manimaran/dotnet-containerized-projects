using BusinesscardsApi.Models;
using Microsoft.EntityFrameworkCore;

namespace BusinesscardsApi.Data;

public class AppDbContext: DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options):base(options)
    {
        
    }
    public DbSet<BusinessCard> BusinessCards { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<BusinessCard>().HasKey(x => x.Id);
    }
}
