using OrdersApi.Models;
using System.Threading.Channels;

namespace OrdersApi.Jobs;

public class OrderProducerService(ChannelWriter<OrderEvent> channelWriter,
                                  ILogger<OrderProducerService> logger) : BackgroundService
{

    private static readonly string[] CustomerNames = [
        "Alice Johnson", "Bob Smith", "Charlie Brown", "Diana Prince", "Ethan Hunt",
        "Fedric Peter"];
    private readonly List<Order> _activeOrders = new();
    private int _orderCounter = 1;
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var random = new Random();
     
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                //  70% chance to produce an order and 30% chance to update existing order status
                if (_activeOrders.Count == 0 || random.NextDouble() < 0.7)
                {
                    // Produce a new order
                    await CreateNewOrder(random, stoppingToken);
                }
                else
                {
                    // Update an existing order status
                    await UpdateExistingOrder(random, stoppingToken);
                }
                
               await Task.Delay(TimeSpan.FromSeconds(random.Next(3,5)), stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
        }
    }

    private async Task CreateNewOrder(Random random, CancellationToken stoppingToken)
    {
        
        var order = new Order(
                    OrderId: $"Ord-{_orderCounter++:D6}",
                    CustomerName: CustomerNames[random.Next(CustomerNames.Length)],
                    Amount: Math.Round((decimal)(random.NextDouble() * 1000 + 10), 2),
                    OrderDate: DateTime.UtcNow);

        _activeOrders.Add(order);
        var orderEvent = new OrderEvent(OrderEventTypes.Created, order);

        await channelWriter.WriteAsync(orderEvent, stoppingToken);

        logger.LogInformation("Created New Order: {OrderId}, Customer: {CustomerName}, Amount: {Amount}, Date: {OrderDate}",
            order.OrderId, order.CustomerName, order.Amount, order.OrderDate);
    }

    private async Task UpdateExistingOrder(Random random, CancellationToken stoppingToken)
    {
        var orderToUpdate = _activeOrders[random.Next(_activeOrders.Count)];
        var newStatus = GetNextStatus(orderToUpdate.Status, random);
        var updatedOrder = orderToUpdate with { Status = newStatus };

        // Replace the old order with the updated one
        var index = _activeOrders.IndexOf(orderToUpdate);
        if (newStatus == OrderStatus.Delivered || newStatus == OrderStatus.Cancelled)
        {
            _activeOrders.RemoveAt(index);
        }
        else
        {
            _activeOrders[index] = updatedOrder;
        }

        var eventType = newStatus == OrderStatus.Cancelled ?
            OrderEventTypes.Cancelled : OrderEventTypes.Updated;

        var orderEvent = new OrderEvent(eventType, updatedOrder);
        await channelWriter.WriteAsync(orderEvent, stoppingToken);

        logger.LogInformation("Updated Order: {OrderId}, New Status: {Status}",
            updatedOrder.OrderId, updatedOrder.Status);
    }

    private static OrderStatus GetNextStatus(OrderStatus currentStatus, Random random)
    {
        return currentStatus switch
        {
            OrderStatus.New => random.NextDouble() < 0.1 ? OrderStatus.Cancelled : OrderStatus.Processing,
            OrderStatus.Processing => random.NextDouble() < 0.1 ? OrderStatus.Cancelled : OrderStatus.Shipped,
            OrderStatus.Shipped => OrderStatus.Delivered,
            _ => currentStatus
        };
    }
}
