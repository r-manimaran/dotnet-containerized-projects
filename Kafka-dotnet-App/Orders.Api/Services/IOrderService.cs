using Orders.Api.DTO;

namespace Orders.Api.Services;

public interface IOrderService
{
    Task<bool> CreateOrder(OrderRequest Order);
}
