
using Confluent.Kafka;
using Microsoft.EntityFrameworkCore.Storage.Json;
using Newtonsoft.Json;
using ProductsApi.Data;
using SharedLib.Models;

namespace ProductsApi.Kafka;

public class KafkaConsumer(IServiceScopeFactory serviceScopeFactory, IConsumer<string, string> consumer) : BackgroundService
{
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return Task.Run(() =>
        {
            _ = ConsumeAsync("order-topic", stoppingToken);
        }, stoppingToken);
    }

    public async Task ConsumeAsync(string topic, CancellationToken stoppingToken)
    {
        //var config = new ConsumerConfig
        //{
        //    GroupId = "order-topic",
        //    BootstrapServers = "localhost:29092",
        //    AutoOffsetReset = AutoOffsetReset.Earliest
        //};
        //using var consumer = new ConsumerBuilder<string,string>(config).Build();

        consumer.Subscribe(topic);
        while (!stoppingToken.IsCancellationRequested)
        {
            var consumeResult = consumer.Consume(stoppingToken);

            var order = JsonConvert.DeserializeObject<Order>(consumeResult.Message.Value);
            using var scope = serviceScopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ProductDbContext>();

            var product =  await dbContext.Products.FindAsync(order.ProductId);
            if (product != null)
            {
                product.Quantity -= order.Quantity;
                await dbContext.SaveChangesAsync();
            }
        }
        consumer.Close();
    }
}
