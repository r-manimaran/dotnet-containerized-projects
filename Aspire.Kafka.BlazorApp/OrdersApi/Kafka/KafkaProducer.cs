using Confluent.Kafka;

namespace OrdersApi.Kafka;

public class KafkaProducer : IKafkaProducer
{
    private readonly IProducer<string,string> _producer;
    public KafkaProducer(IProducer<string, string> producer)
    {
        //var config = new ConsumerConfig
        //{
        //    GroupId = "order-group",
        //    BootstrapServers ="localhost:29092",
        //    AutoOffsetReset = AutoOffsetReset.Earliest
        //};
        _producer = producer;
    }
    public Task ProduceAsync(string topic, Message<string, string> message)
    {
       return _producer.ProduceAsync(topic, message);
    }
}
