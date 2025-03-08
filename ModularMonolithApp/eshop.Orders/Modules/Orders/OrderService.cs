using eshop.Orders.Modules.Orders.Models;
using eshop.Orders.Modules.Orders.PublicApi;
using Microsoft.EntityFrameworkCore;

namespace eshop.Orders.Modules.Orders;

public class OrderService : IOrderService
{
    private readonly OrderDbContext _dbContext;

    public OrderService(OrderDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<Order> CreateOrder(CreateOrderDto newOrder)
    {
        var order = new Order
        {
            Id = Guid.NewGuid(),
            CustomerName = newOrder.CustomerName,
            ShippingAddress = newOrder.ShippingAddress,
            ProductName = newOrder.ProductName,
            Quantity = newOrder.Quantity,
            TotalPrice = newOrder.TotalPrice,
            OrderDate = DateTime.UtcNow
        };

        _dbContext.Orders.Add(order);
        await _dbContext.SaveChangesAsync();

        return order;
    }

    public async Task<Order?> GetOrder(Guid id)
    {
        var order = await _dbContext.Orders.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        return order;
    }
}
