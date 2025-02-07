using Microsoft.EntityFrameworkCore;
using SharedLib.Models;

namespace OrdersApi.Data;

public class OrderDbContext:DbContext
{
    public OrderDbContext(DbContextOptions<OrderDbContext> options):base(options)
    {
        Database.EnsureCreated();
    }

    public DbSet<Order> Orders { get; set; }

}
