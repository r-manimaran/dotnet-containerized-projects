using Microsoft.EntityFrameworkCore;

namespace eshop.Orders.Modules.Shipping;

public class ShippingDbContext: DbContext
{
    public ShippingDbContext(DbContextOptions<ShippingDbContext> options):base(options)
    {
        
    }
}
