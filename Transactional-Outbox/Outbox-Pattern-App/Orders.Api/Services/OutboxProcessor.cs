using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace Orders.Api.Services;

internal sealed class OutboxProcessor
{
    private readonly AppDbContext _dbContext;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<OutboxProcessor> _logger;

    private const int BatchSize = 10;
    public OutboxProcessor(AppDbContext dbContext,
                           IPublishEndpoint publishEndpoint,
                           ILogger<OutboxProcessor> logger)
    {
        _dbContext = dbContext;
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }

    public async Task<int> Execute(CancellationToken cancellationToken =default)
    {
        // Steps needs to be performed.
        // 1. Read the OutboxMessages tables for unprocessed messages
        // 2. Publish it to RabbitMQ using IPublishEndpoint
        // 3. Update back the OutboxMessages for successful or error info on publishing
        _logger.LogInformation("Processing outbox messages");
        using var transaction = await _dbContext.Database.BeginTransactionAsync();

        //Fetch batch of messages from outbox table
        var outboxMessages = await _dbContext.OutboxMessages
            .Where(x => x.ProcessedOnUtc == null || x.Error != null)
            .OrderBy(x => x.OccuredOnUtc)
            .Take(BatchSize)
            .ToListAsync();

        foreach (var outboxMessage in outboxMessages)
        {
            try
            {
                // Deserialize the message content
                var messageType = Message.Contracts.AssemblyReference.Assembly.GetType(outboxMessage.Type)!;

                var message = System.Text.Json.JsonSerializer.Deserialize(outboxMessage.Content, messageType);
                
                // Publish the message to RabbitMQ
                await _publishEndpoint.Publish(message, messageType, cancellationToken);

                // Update the OutboxMessage as processed
                outboxMessage.ProcessedOnUtc = DateTime.UtcNow;
                outboxMessage.Error = null;
                _logger.LogInformation("Processed outbox message {OutboxMessageId}", outboxMessage.Id);
            }
            catch (Exception ex)
            {
                // Update the OutboxMessage as error
                outboxMessage.ProcessedOnUtc = DateTime.UtcNow;
                outboxMessage.Error = ex.ToString();
                _logger.LogError(ex, "Error processing outbox message");
            }
            await _dbContext.SaveChangesAsync(cancellationToken);
          
        }

        if (outboxMessages.Any())
        {
            _logger.LogInformation("Processed {Count} outbox messages", outboxMessages.Count);
        }

        // commit the transacation
        await transaction.CommitAsync(cancellationToken);

        return outboxMessages.Count;
    }
}
