using Blogging.Api.Data.Configurations;
using Blogging.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Blogging.Api.Data;

public class BlogDbContext: DbContext
{
    public BlogDbContext(DbContextOptions<BlogDbContext> options): base(options)
    {
        
    }

    public DbSet<Post> Posts { get; set; }
    public DbSet<Category> Categories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new PostConfiguration());
        modelBuilder.ApplyConfiguration(new CategoryConfiguration());
    }
}
