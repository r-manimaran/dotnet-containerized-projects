using Microsoft.EntityFrameworkCore;
using Npgsql;
using Webhooks.Processing.Data;
using Webhooks.Processing.Extensions;
using Webhooks.Processing.OpenTelemetry;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddControllers();

builder.Services.AddOpenApi();

builder.Services.AddDbContext<WebhookDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("webhooks")));

builder.AddMassTransitRabbitMQ();

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
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.Run();
