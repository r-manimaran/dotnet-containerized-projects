using Aspire.AI.SQLServer.Models;
using Microsoft.EntityFrameworkCore;

namespace Aspire.AI.SQLServer.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        
    }
    public DbSet<User> Users { get; set; }
    public DbSet<Todo> Todos { get; set; }
    override protected void OnModelCreating(ModelBuilder modelBuilder)
    {
       modelBuilder.ApplyConfiguration(new TodoConfiguration());
       modelBuilder.ApplyConfiguration(new UserConfiguration());

       //modelBuilder.Entity<Todo>().ComplexProperty(b => b.Metadata, b => b.ToJson());
    }
}
