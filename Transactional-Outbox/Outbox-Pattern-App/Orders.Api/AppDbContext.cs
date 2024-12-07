using Microsoft.EntityFrameworkCore;
using Orders.Api.Models;

namespace Orders.Api
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) :base(options) { }

        public DbSet<Order> Orders { get; set; }

        // For Outbox Pattern
        public DbSet<OutboxMessage> OutboxMessages { get; set; }
        
    }
}
