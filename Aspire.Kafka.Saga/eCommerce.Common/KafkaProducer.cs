using Confluent.Kafka;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eCommerce.Common;

public class KafkaProducer : IKafkaProducer
{
    private readonly IProducer<string, string> _producer;
    public KafkaProducer(IProducer<string, string> producer)
    {
        //var config = new ConsumerConfig
        //{
        //    GroupId = "order-group",
        //    AutoOffsetReset = AutoOffsetReset.Earliest,
        //    BootstrapServers = "localhost:29092"
        //};
        //_producer = new ProducerBuilder<string, string>(config).Build();
        _producer = producer;
    }
    public async Task ProduceAsync(string topic, object message)
    {
        try
        {
            var kafkaMessage = new Message<string, string> { Value = JsonConvert.SerializeObject(message) };

            await _producer.ProduceAsync(topic, kafkaMessage);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }
}

public interface IKafkaProducer
{
    Task ProduceAsync(string topic, object message);
}
