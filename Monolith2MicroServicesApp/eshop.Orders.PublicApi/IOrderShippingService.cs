
namespace eshop.Orders.PublicApi;

public interface IOrderShippingService
{
    Task<OrderShippingInfo?> GetOrderForShippingAsync(Guid orderId);
}
