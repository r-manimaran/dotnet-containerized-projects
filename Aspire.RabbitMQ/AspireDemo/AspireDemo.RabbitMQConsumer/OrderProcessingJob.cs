using AspireDemo.Common.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text.Json;

namespace AspireDemo.RabbitMQConsumer;

public class OrderProcessingJob : BackgroundService
{
    private readonly ILogger<OrderProcessingJob> _logger;
    private readonly IConfiguration _configuration;
    private readonly IServiceProvider _serviceProvider;

    private  IConnection? _messageConnection;
    private  IModel? _messageChannel;

    public OrderProcessingJob(ILogger<OrderProcessingJob> logger,
                              IConfiguration configuration,
                              IServiceProvider serviceProvider)
    {
        _logger = logger;
        _configuration = configuration;
        _serviceProvider = serviceProvider;
    }
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        const string configName = "RabbitMQ:QueueName";
        string queueName = _configuration[configName] ?? "orders";

        _messageConnection = _serviceProvider.GetRequiredService<IConnection>();

        _messageChannel = _messageConnection!.CreateModel();
        _messageChannel.QueueDeclare(queueName, exclusive: false);

        var consumer = new EventingBasicConsumer(_messageChannel);
        consumer.Received += ProcessMessageAsync;

        _messageChannel.BasicConsume(
                queue:queueName,
                consumer:consumer,
                autoAck:true);
        return Task.CompletedTask;
    }
    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        await base.StopAsync(cancellationToken);
        _messageChannel?.Dispose();
    }

    private void ProcessMessageAsync(object? sender, BasicDeliverEventArgs args)
    {
        _logger.LogInformation($"Processing Order at :{DateTime.UtcNow} with messageId:{args.BasicProperties.MessageId}");
        var message = args.Body;
        var model=JsonSerializer.Deserialize<Order>(message.Span);
        _logger.LogInformation($"Message Received and Processed:{model!.Message} and Amount: {model!.price}");
    }
}
