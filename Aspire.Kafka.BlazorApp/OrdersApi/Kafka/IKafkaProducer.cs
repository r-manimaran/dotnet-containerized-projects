using Confluent.Kafka;

namespace OrdersApi.Kafka;

public interface IKafkaProducer
{
    Task ProduceAsync(string topic, Message<string,string> message);
}
