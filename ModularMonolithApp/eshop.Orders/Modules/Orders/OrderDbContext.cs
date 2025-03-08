using eshop.Orders.Modules.Orders.Models;
using Microsoft.EntityFrameworkCore;

namespace eshop.Orders.Modules.Orders
{
    public class OrderDbContext:DbContext
    {
        public OrderDbContext(DbContextOptions<OrderDbContext> options):base(options)
        {
            
        }
        public DbSet<Order> Orders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Order>()
                .ToTable("orders", "orders");  // (tableName, schemaName)
        }
    }
}
