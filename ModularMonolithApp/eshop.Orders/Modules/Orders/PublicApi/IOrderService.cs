using eshop.Orders.Modules.Orders.Models;

namespace eshop.Orders.Modules.Orders.PublicApi;

public interface IOrderService
{
    Task<Order> CreateOrder(CreateOrderDto newOrder);
    Task<Order?> GetOrder(Guid id);

    Task<OrderShippingInfo?> GetOrderForShippingAsync(Guid orderId);
}
