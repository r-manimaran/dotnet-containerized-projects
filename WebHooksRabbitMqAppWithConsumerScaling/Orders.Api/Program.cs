using Microsoft.EntityFrameworkCore;
using Npgsql;
using Orders.Api.Data;
using Orders.Api.Endpoints;
using Orders.Api.Extensions;
using Orders.Api.OpenTelemetry;
using Orders.Api.Repositories;
using Orders.Api.Services;
using System.Threading.Channels;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();


builder.Services.AddOpenApi();
builder.Services.AddSingleton<InMemoryOrderRepository>();
builder.Services.AddSingleton<InMemorySubscriptionRepository>();
builder.Services.AddHttpClient<WebhookDispatcher>();

// Added for Persistance data
builder.Services.AddHttpClient();
builder.Services.AddScoped<WebhookDispatcherUsingDB>();

builder.Services.AddDbContext<WebhookDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("webhooks")));

/*
// Added BackgroundJob Service to process the Subscriptions
builder.Services.AddHostedService<WebhookProccesor>();

// Added the Channel with max 100 message and On Full, set it to Wait
builder.Services.AddSingleton(_ =>
{
    return Channel.CreateBounded<WebhookDispatch>(new BoundedChannelOptions(100)
    {
        FullMode = BoundedChannelFullMode.Wait
    });
});
*/

// Add MassTransit Configuration
builder.AddMassTransitRabbitMQ();

// Add the custom ActivitySource to the OpenTelemetry
builder.Services.AddOpenTelemetry()
    .WithTracing(tracking =>
    {
        tracking.AddSource(DiagnosticConfig.ActivitySource.Name)
                .AddSource(MassTransit.Logging.DiagnosticHeaders.DefaultListenerName)
                .AddNpgsql();
    });

var app = builder.Build();

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "OpenAPI V1");
    });

    // Apply the Migration
    await app.ApplyMigrationAsync();

}

app.UseHttpsRedirection();

// Endpoints using InMemory
// app.MapOrderEndpoints();

// Endpoints using Postgres persistance
app.MapPersistentOrderEndpoints();

// Endpoints using InMemory
// app.MapWebhookEndpoints();

// Endpoint using Postgres persistance
app.MapPersistantWebhookEndpoints();

app.Run();


