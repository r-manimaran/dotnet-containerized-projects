using OrderProcessingService.Models;

namespace OrderProcessingService.Services;

public interface IOrderRepository 
{
    Order GetOrderById(Guid orderId);
    void AddOrder(Order order);
}
