using Confluent.Kafka;
using eCommerce.Common;
using Newtonsoft.Json;
using ProductsApi.Data;
using SharedLib.Enums;
using SharedLib.Models;

namespace ProductsApi.Kafka;

public class ProductConsumer(IServiceProvider serviceProvider, 
                            IKafkaProducer kafkaProducer, 
                            IConsumer<string,string> consumer) : KafkaConsumer(topics, consumer)
{
    private static readonly string[] topics = [KafkaTopics.ORDER_CREATED, KafkaTopics.PAYMENT_FAILED];

    private ProductDbContext GetDbContext()
    {
        var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ProductDbContext>();
        return dbContext;
    }
    protected override async Task ConsumeAsync(ConsumeResult<string, string> consumeResult)
    {
        await base.ConsumeAsync(consumeResult);
        switch(consumeResult.Topic)
        {
            case KafkaTopics.ORDER_CREATED:
                await HandleOrderCreated(consumeResult.Message.Value);
                break;
            case KafkaTopics.PAYMENT_FAILED:
                await HandlePaymentFailed(consumeResult.Message.Value);
                break;
        }
    }
    public async Task HandleOrderCreated(string message)
    {
        var orderMessage = JsonConvert.DeserializeObject<OrderMessage>(message);
        var isReserved = await ReserveProducts(orderMessage);
        if (isReserved)
        {
            await kafkaProducer.ProduceAsync(KafkaTopics.PRODUCTS_RESERVED, orderMessage);
        }
        else
        {
            await kafkaProducer.ProduceAsync(KafkaTopics.PRODUCTS_RESERVATION_FAILED, orderMessage);
        }
    }


    private async Task HandlePaymentFailed(string message)
    {
        var orderMessage = JsonConvert.DeserializeObject<OrderMessage>(message);
        using var dbContext = GetDbContext();
        var product = await dbContext.Products.FindAsync(orderMessage.ProductId);
        if (product != null) // compenensation task to revert the quantity of products reserved
        {
            product.Quantity += orderMessage.Quantity;
            await dbContext.SaveChangesAsync();            
        }
        await kafkaProducer.ProduceAsync(KafkaTopics.PRODUCTS_RESERVATION_CANCELED, orderMessage);
    }

   
    public async Task<bool> ReserveProducts(OrderMessage orderMessage)
    {
        using var dbContext = GetDbContext();
        var product = await dbContext.Products.FindAsync(orderMessage.ProductId);
        if (product!=null && product.Quantity >=orderMessage.Quantity)
        {
            product.Quantity -= orderMessage.Quantity;
            await dbContext.SaveChangesAsync();
            return true;
        }
        return false;
    }
}
