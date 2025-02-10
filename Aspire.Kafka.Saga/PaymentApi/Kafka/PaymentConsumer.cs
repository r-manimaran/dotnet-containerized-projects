using Confluent.Kafka;
using eCommerce.Common;
using Newtonsoft.Json;
using SharedLib.Enums;
using SharedLib.Models;

namespace PaymentApi.Kafka;

public class PaymentConsumer(IConsumer<string,string> consumer,
                             IKafkaProducer kafkaProducer) :KafkaConsumer(topics,consumer)
{
   
    private static readonly string[] topics = [KafkaTopics.PRODUCTS_RESERVED];

    protected override async Task ConsumeAsync(ConsumeResult<string, string> consumeResult)
    {
        await base.ConsumeAsync(consumeResult);

        switch (consumeResult.Topic)
        {
            case KafkaTopics.PRODUCTS_RESERVED:
                await HandleProductReserved(consumeResult.Message.Value);
                break;
        }
    }

    public async Task HandleProductReserved(string message)
    {
        var orderMessage = JsonConvert.DeserializeObject<OrderMessage>(message);
        var isPaymentProcessed = ProcessPayment(orderMessage);

        if (isPaymentProcessed)
        {
            // Produce a message to payment-Processed topic
            await kafkaProducer.ProduceAsync(KafkaTopics.PAYMENT_PROCESSED, orderMessage);
        }
        else
        {
            await kafkaProducer.ProduceAsync(KafkaTopics.PAYMENT_FAILED, orderMessage);
        }
    }

    public bool ProcessPayment(OrderMessage orderMessage)
    {
        // Logic to process Payment
        // For testing create a Random payment failure
                
        var random = new Random();
        var randomNumber = random.Next(1, 10);

        if (randomNumber % 2 == 0)
        {           
            return false;
        }

        return true;
    }
}