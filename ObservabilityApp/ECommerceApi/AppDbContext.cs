using ECommerceApi.Models;
using Microsoft.EntityFrameworkCore;

namespace ECommerceApi;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options):base(options)
    {
        
    }

    public DbSet<Customer> Customers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Customer>().HasKey(x => x.Id);

        modelBuilder.Entity<Customer>().Property(x => x.Name).IsRequired().HasMaxLength(100);

        modelBuilder.Entity<Customer>().Property(x => x.Email).IsRequired().HasMaxLength(100);
    }
}
