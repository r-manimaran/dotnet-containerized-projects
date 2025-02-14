using Orders.Api.Models;

namespace Orders.Api.Repositories;

internal sealed class InMemoryOrderRepository
{
    private readonly ILogger<InMemoryOrderRepository> _logger;
    public readonly List<Order> _inMemoryOrders = new List<Order>();
    public InMemoryOrderRepository(ILogger<InMemoryOrderRepository> logger)
    {
        _logger = logger;
    }

    public void AddOrder(Order order)
    {
        _logger.LogInformation("Adding new Order {order}", order);
        _inMemoryOrders.Add(order);
    }

    public IReadOnlyList<Order> GetAll()
    {
        _logger.LogInformation("Returning Orders");
        return _inMemoryOrders.AsReadOnly();
    }
}
