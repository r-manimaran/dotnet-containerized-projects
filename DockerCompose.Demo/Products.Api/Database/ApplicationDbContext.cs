using Microsoft.EntityFrameworkCore;
using Products.Api.Entities;

namespace Products.Api.Database
{
    public class ApplicationDbContext:DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options):base(options) { }

        public DbSet<Product> Products { get; set; }
       
    }
}
