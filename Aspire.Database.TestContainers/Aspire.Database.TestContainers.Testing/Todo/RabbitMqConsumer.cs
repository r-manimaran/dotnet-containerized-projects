using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aspire.Database.TestContainers.Testing;

public class RabbitMqConsumer
{
    private readonly ConnectionFactory _factory;
    public RabbitMqConsumer(string connectionString)
    {
        _factory = new ConnectionFactory()
        {
            Uri = new Uri(connectionString)
        };
    }

    public void BindQueue(string exchange, string queueName)
    {
        using var connection = _factory.CreateConnection();
        using var channel = connection.CreateModel();

        channel.ExchangeDeclare(exchange: exchange, type: ExchangeType.Fanout,
                    durable: true);
        var queueResult = channel.QueueDeclare(queueName, durable: true, exclusive: false,autoDelete:false,arguments:null);
        channel.QueueBind(queue: queueResult.QueueName, exchange: exchange, routingKey: string.Empty);

    }

    public async Task<bool> TryToConsumeAsync(string queueName, TimeSpan timeout)
    {
        var messageReceived = new TaskCompletionSource<bool>();
        using var connection = _factory.CreateConnection(); 
        using var channel = connection.CreateModel();

        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += (sender, e) =>
        {
            messageReceived.SetResult(true);
        };
        channel.BasicConsume(queueName, true, consumer);

        var timeoutTask = Task.Delay(timeout);
        var completedTask = await Task.WhenAny(messageReceived.Task, timeoutTask);

        return completedTask == messageReceived.Task;
    }
}
