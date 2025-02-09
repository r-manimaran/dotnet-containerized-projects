using Confluent.Kafka;
using eCommerce.Common;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using OrdersApi.Data;
using SharedLib.Enums;
using SharedLib.Models;

namespace OrdersApi.Kafka;

public class OrderConsumer(IConsumer<string, string> consumer,
                    IServiceProvider serviceProvider):KafkaConsumer(topics, consumer)
{
    private static readonly string[] topics = [KafkaTopics.PAYMENT_PROCESSED, 
                                               KafkaTopics.PRODUCTS_RESERVATION_FAILED,
                                               KafkaTopics.PRODUCTS_RESERVATION_CANCELED];

    private OrderDbContext GetDbContext()
    {
        var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<OrderDbContext>();
        return dbContext;
    }
    protected override async Task ConsumeAsync(ConsumeResult<string, string> consumeResult)
    {
        await base.ConsumeAsync(consumeResult);
        var orderMessage = JsonConvert.DeserializeObject<OrderMessage>(consumeResult.Message.Value);
        switch (consumeResult.Topic)
        {
            case KafkaTopics.PAYMENT_PROCESSED:
                await HandleConfirmOrder(orderMessage);
                break;
            case KafkaTopics.PRODUCTS_RESERVATION_FAILED:
                await HandleCancelOrder(orderMessage);
                break;
            case KafkaTopics.PRODUCTS_RESERVATION_CANCELED:
                await HandleCancelOrder(orderMessage);
                break;
        }

    }

    private async Task HandleConfirmOrder(OrderMessage orderMessage)
    {
        using var dbContext = GetDbContext();
        var order = await dbContext.Orders.FindAsync(orderMessage.OrderId);
        if(order != null)
        {
            order.Status = "Confirm";
            await dbContext.SaveChangesAsync();
        }
    }

    private async Task HandleCancelOrder(OrderMessage? orderMessage)
    {
        using var dbContext = GetDbContext();
        var order = await dbContext.Orders.FindAsync(orderMessage.OrderId);
        if (order != null)
        {
            order.Status = "Cancelled";
            await dbContext.SaveChangesAsync();
        }
    }
}
