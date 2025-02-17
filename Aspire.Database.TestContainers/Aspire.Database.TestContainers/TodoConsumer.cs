
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace Aspire.Database.TestContainers;

public class TodoConsumer : BackgroundService
{
    private readonly ILogger<TodoConsumer> _logger;
    private readonly IConfiguration _config;
    private IConnection? _messageConnection;
    private IModel? _messageChannel;
    private readonly IServiceProvider _serviceProvider;
    private EventingBasicConsumer consumer;
    public TodoConsumer(ILogger<TodoConsumer> logger, IConfiguration config, IServiceProvider serviceProvider,
        IConnection? messageConnection)
    {
        _logger = logger;
        _config = config;
        _serviceProvider = serviceProvider;
    }
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        string queueName = "todoqueue";
        _messageConnection = _serviceProvider.GetRequiredService<IConnection>();

        _messageChannel = _messageConnection.CreateModel();
        _messageChannel.QueueDeclare(queue: queueName, durable:false, exclusive:false, autoDelete:false, arguments:null);

        consumer = new EventingBasicConsumer(_messageChannel);
        consumer.Received += ProcessMessageAsync;

        _messageChannel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);
        return Task.CompletedTask;
    }
    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        await base.StopAsync(cancellationToken);
        consumer.Received -= ProcessMessageAsync;
        _messageChannel?.Dispose();
    }

    private void ProcessMessageAsync(object? sender, BasicDeliverEventArgs args)
    {
        string messageText = Encoding.UTF8.GetString(args.Body.ToArray());
        _logger.LogInformation("Received Message at {now}. Message Text:{text}",DateTime.Now,messageText);
        var message = args.Body;
    }
}
