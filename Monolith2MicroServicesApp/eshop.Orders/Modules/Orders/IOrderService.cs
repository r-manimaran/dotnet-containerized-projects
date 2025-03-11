using eshop.Orders.Modules.Orders.Models;
using eshop.Orders.PublicApi;

namespace eshop.Orders.Modules.Orders
{
    public interface IOrderService
    {
        Task<Order> CreateOrder(CreateOrderDto newOrder);
        Task<Order?> GetOrder(Guid id);
        Task<OrderShippingInfo?> GetOrderForShippingAsync(Guid orderId);
    }
}