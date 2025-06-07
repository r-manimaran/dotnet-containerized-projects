using AppreciateAppApi.Models;
using Microsoft.EntityFrameworkCore;

namespace AppreciateAppApi.Data;

public class AppDbContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<Employee> Employees { get; set; } = null!;
    public DbSet<Item> Items { get; set; } = null!;
    public DbSet<Category> Categories { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Employee>().HasKey(e => e.Id);

        modelBuilder.Entity<Category>().HasKey(c => c.Id);

        modelBuilder.Entity<Item>(entity =>
        {
            entity.HasKey(i => i.Id);

            entity.HasOne(i => i.Category)
              .WithMany()
              .HasForeignKey(i => i.CategoryId)
              .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(i => i.Sender)
                .WithMany()
                .HasForeignKey(i => i.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

        });
        
        // Configure many-to-many relationship for Receivers
        modelBuilder.Entity<Item>()
                    .HasMany(i => i.Receivers)
                    .WithMany()
                    .UsingEntity(j => j.ToTable("ItemReceivers"));

        //modelBuilder.Entity<Sender>().HasNoKey();

        // modelBuilder.Entity<Receiver>().HasNoKey();
    }
}
