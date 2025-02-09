using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Confluent.Kafka.ConfigPropertyNames;

namespace eCommerce.Common;

public class KafkaConsumer : BackgroundService
{

    private IConsumer<string,string> _consumer;
    public KafkaConsumer(string[] topics, IConsumer<string,string> consumer)
    {
       _consumer = consumer;
       _consumer.Subscribe(topics);
    }
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return Task.Run(() =>
        {
            _ = HandleConsume("order-topic", stoppingToken);
        }, stoppingToken);
    }

    public async Task HandleConsume(string topic, CancellationToken stoppingToken)
    {

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var consumeResult = _consumer.Consume(stoppingToken);
                await ConsumeAsync(consumeResult);
                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing message: {ex.Message}");

            }
        }
        _consumer.Close();
    }

    protected virtual Task ConsumeAsync(ConsumeResult<string,string> consumeResult)
    {
        return Task.CompletedTask;
    }
}
