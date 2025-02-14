using MassTransit;
using Webhooks.Processing.Services;

namespace Webhooks.Processing.Extensions;

public static class AppExtensions
{
    public static async void AddMassTransitRabbitMQ(this WebApplicationBuilder builder)
    {
        builder.Services.AddMassTransit(busConfig =>
        {
            busConfig.SetKebabCaseEndpointNameFormatter();

            busConfig.AddConsumer<WebhookDispatchedConsumer>();
            busConfig.AddConsumer<WebhookTriggeredConsumer>();

            busConfig.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(builder.Configuration.GetConnectionString("RabbitMQ"));

                cfg.ConfigureEndpoints(context);
            });

        });

    }
}
