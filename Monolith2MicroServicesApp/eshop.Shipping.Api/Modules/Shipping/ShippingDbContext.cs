using eshop.Orders.Modules.Shipping.Models;
using Microsoft.EntityFrameworkCore;

namespace eshop.Orders.Modules.Shipping;

public class ShippingDbContext: DbContext
{
    public ShippingDbContext(DbContextOptions<ShippingDbContext> options):base(options)
    {
        
    }
    public DbSet<ShipmentRecord> ShipmentRecords { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ShipmentRecord>()
                .ToTable("shipment_records", "shipping");  // (tableName, schemaName)

        modelBuilder.Entity<ShipmentRecord>().HasKey(x => x.Id);
        
    }
}
