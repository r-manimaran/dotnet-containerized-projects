using MassTransit;
using Microsoft.EntityFrameworkCore;
using Orders.Api.Data;
using Orders.Api.Services;

namespace Orders.Api.Extensions;

public static class WebAppExtensions
{
    public static async Task ApplyMigrationAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var db =scope.ServiceProvider.GetRequiredService<WebhookDbContext>();

        await db.Database.MigrateAsync();
    }

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
