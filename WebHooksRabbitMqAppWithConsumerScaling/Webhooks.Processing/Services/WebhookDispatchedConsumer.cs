using MassTransit;
using MassTransit.Courier.Contracts;
using Microsoft.EntityFrameworkCore;
using Webhooks.Contracts;
using Webhooks.Processing.Data;

namespace Webhooks.Processing.Services;

internal sealed class WebhookDispatchedConsumer(WebhookDbContext dbContext) : IConsumer<WebhookDispatched>
{

    public async Task Consume(ConsumeContext<WebhookDispatched> context)
    {
        var message = context.Message;

        var subscriptions = await dbContext.WebhookSubscriptions
                            .AsNoTracking()
                            .Where(sub => sub.EventType == message.EventType)
                            .ToListAsync();

        /*   foreach (var subscription in subscriptions)
           {
               await context.Publish(new WebhookTriggered(
                   subscription.Id,
                   subscription.EventType,
                   subscription.WebhookUrl,
                   message.Data));
           } */

        // Batch publish instead of above foreach loop
        await context.PublishBatch(subscriptions.Select(subscription => new WebhookTriggered(
                        subscription.Id,
                        subscription.EventType,
                        subscription.WebhookUrl,
                        message.Data)));
    }
}
