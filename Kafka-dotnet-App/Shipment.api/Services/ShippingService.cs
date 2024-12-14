using Confluent.Kafka;
using Message.Contract;
using System.Collections.Concurrent;
using System.Text.Json;

namespace Shipment.api.Services
{
    public class ShippingService : IShippingService
    {
        private readonly ILogger<ShippingService> _logger;
        private readonly IConsumer<Null, string> _consumer;
        private const string NewOrderTopic = "new-order-topic";
        private List<OrderReceivedMessage> _messages = new List<OrderReceivedMessage>();
        public ShippingService(ILogger<ShippingService> logger,
                               IConsumer<Null,string> consumer)
        {
            _logger = logger;
            _consumer = consumer;
        }

        public async Task ConsumeOrders()
        {
            await Task.Delay(10);
            // In Subscribe we are passing a collection. If there is more than one topic, we can pass through those.
            _consumer.Subscribe([NewOrderTopic]);

            while (true)
            {
                var response = _consumer.Consume();
                if (!string.IsNullOrEmpty(response.Message.Value))
                {
                    var newOrder = JsonSerializer.Deserialize<OrderReceivedMessage>(response.Message.Value);
                    _messages.Add(newOrder);
                    _logger.LogInformation("Received the new-order message from Kafka..{message}", response.Message.Value);
                    _logger.LogInformation($"Total Messages so far:{_messages.Count}");

                }
            }

        }
    }
}
