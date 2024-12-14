using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Products.Api.Models;

    public class AppDbContext : DbContext
    {
        public AppDbContext (DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Products.Api.Models.Product> Product { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>()
                    .Property(p => p.Price)
                    .HasColumnType("decimal(18,2)");
        modelBuilder.Entity<Product>()
                    .Property(p => p.Price)
                    .HasPrecision(18, 2);
        modelBuilder.Entity<Product>()
                    .Property(p => p.Price)
                    .HasConversion<decimal>();
    }
}
