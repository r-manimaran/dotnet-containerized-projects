using Shared.Contracts.DTOs;
using Shared.Contracts.Models;

namespace Orders.Api.Repositories;

public interface IOrder
{
    Task<ServiceResponse> AddOrderAsync(OrderRequest order);
    Task<List<Order>> GetAllOrdersAsync();
    Task<OrderSummary> GetOrderSummaryAsync();
}
