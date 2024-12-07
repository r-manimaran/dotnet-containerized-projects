
namespace Orders.Api.Services;

public class OutboxBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<OutboxBackgroundService> _logger;
    private const int OutboxProcessorFrequency = 7;
    public OutboxBackgroundService(IServiceScopeFactory serviceScopeFactory,
                                   ILogger<OutboxBackgroundService> logger)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            _logger.LogInformation("Starting OutboxBackgroundService...");

            while (!stoppingToken.IsCancellationRequested) 
            { 
                using var scope = _serviceScopeFactory.CreateScope();
                var outboxProcessor = scope.ServiceProvider.GetRequiredService<OutboxProcessor>();

                await outboxProcessor.Execute(stoppingToken);

                //Simulate running Outbox processor every N seconds
                await Task.Delay(TimeSpan.FromSeconds(OutboxProcessorFrequency), stoppingToken);

            }
        }
        catch(OperationCanceledException) 
        {
            _logger.LogInformation("OutboxBackgroundService cancelled...");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occured in OutboxBackgroundService..");
        }
        finally
        {
            _logger.LogInformation("Finished Processing OutboxBackgroundService..");
        }
    }
}
