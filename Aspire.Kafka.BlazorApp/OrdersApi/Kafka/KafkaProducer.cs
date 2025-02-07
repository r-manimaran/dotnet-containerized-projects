using Confluent.Kafka;

namespace OrdersApi.Kafka;

public class KafkaProducer : IKafkaProducer
{
    private readonly IProducer<string,string> _producer;
    public KafkaProducer(IProducer<string, string> producer)
    {
        _producer = producer;
    }
    public Task ProduceAsync(string topic, Message<string, string> message)
    {
       return _producer.ProduceAsync(topic, message);
    }
}
