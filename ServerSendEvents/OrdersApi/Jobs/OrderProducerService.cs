using OrdersApi.Models;

namespace OrdersApi.Jobs;

public class OrderProducerService(ILogger<OrderProducerService> logger) : BackgroundService
{

    private static readonly string[] CustomerNames = [
        "Alice Johnson", "Bob Smith", "Charlie Brown", "Diana Prince", "Ethan Hunt",
        "Fedric Peter"];


    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var random = new Random();
        var orderCounter = 1;
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var order = new Order(
                    OrderId: $"Ord-{orderCounter++:D6}",
                    CustomerName: CustomerNames[random.Next(CustomerNames.Length)],
                    Amount: Math.Round((decimal)(random.NextDouble() * 1000 + 10), 2),
                    OrderDate: DateTime.UtcNow);

                logger.LogInformation("Produced Order: {OrderId}, Customer: {CustomerName}, Amount: {Amount}, Date: {OrderDate}",
                    order.OrderId, order.CustomerName, order.Amount, order.OrderDate);

               await Task.Delay(TimeSpan.FromSeconds(random.Next(3,5)), stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
        }
    }
}
