using Microsoft.EntityFrameworkCore;
using Shipping.Api.Models;

namespace Shipping.Api
{
    public class AppDbContext:DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> dbContext) :base(dbContext)
        {
            
        }

        public DbSet<Shipment> Shipments { get; set; }
    }
}
