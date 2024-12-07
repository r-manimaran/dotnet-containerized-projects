using Message.Contracts;
using Orders.Api.DTOs;

namespace Orders.Api.Services
{
    public interface IOrderService
    {
        Task<OrderPublish> CreateNewOrderAsync(OrderRequest orderRequest);
        Task<OrderPublish> GetOrderAsync(Guid  orderId);
    }
}
