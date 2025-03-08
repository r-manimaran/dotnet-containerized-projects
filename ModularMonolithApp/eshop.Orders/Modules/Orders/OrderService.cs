using eshop.Orders.Modules.Orders.Models;
using eshop.Orders.Modules.Orders.PublicApi;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace eshop.Orders.Modules.Orders;

public class OrderService : IOrderService
{
    private readonly OrderDbContext _dbContext;
    private readonly IPublishEndpoint _publishEndpoint;

    public OrderService(OrderDbContext dbContext, IPublishEndpoint publishEndpoint)
    {
        _dbContext = dbContext;
        _publishEndpoint = publishEndpoint;
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

        // Publish the event using Mass Transit
        var orderCreatedEvent = new OrderCreatedIntegrationEvent { OrderId = order.Id };
        await _publishEndpoint.Publish(orderCreatedEvent);

        return order;
    }

    public async Task<Order?> GetOrder(Guid id)
    {
        var order = await _dbContext.Orders.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        return order;
    }

    public async Task<OrderShippingInfo?> GetOrderForShippingAsync(Guid orderId)
    {
        var order = await _dbContext.Orders.AsNoTracking().FirstOrDefaultAsync(x => x.Id == orderId);
        if(order!=null)
        {
            OrderShippingInfo info = new OrderShippingInfo
            {
                OrderId = order.Id,
                ShippingAddress = order.ShippingAddress
            };
            return info;
        }
        return null;
    }
}
