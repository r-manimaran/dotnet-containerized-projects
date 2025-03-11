using eshop.Orders;
using eshop.Orders.Modules.Shipping;
using eshop.Orders.PublicApi;
using eshop.Shipping.Api.Services;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Npgsql;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Polly;
using Polly.Extensions.Http;
using System;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<DatabaseInitializer>();

builder.Services.AddSingleton(_ =>
{
    return new NpgsqlDataSourceBuilder(builder.Configuration.GetConnectionString("Database")).Build();
});


builder.Services.AddDbContext<ShippingDbContext>(options =>
    options.EnableSensitiveDataLogging()
            .UseNpgsql(builder.Configuration.GetConnectionString("Database"),
             x => x.MigrationsHistoryTable(HistoryRepository.DefaultTableName, "shipping"))
            .UseSnakeCaseNamingConvention());

builder.Services.AddTransient<IOrderShippingService, OrderService>();

builder.Services.AddOpenTelemetry()
       .ConfigureResource(r => r.AddService("eshop.Shipping.Api"))
       .WithTracing(tracing =>
            tracing.
                   AddNpgsql()
                   .AddHttpClientInstrumentation()
                   .AddAspNetCoreInstrumentation()
                   .AddEntityFrameworkCoreInstrumentation()
                   .AddSource(MassTransit.Logging.DiagnosticHeaders.DefaultListenerName))
       .UseOtlpExporter();

// Added Mass Transit
builder.Services.AddMassTransit(x =>
{
    x.SetKebabCaseEndpointNameFormatter();
    x.AddConsumer<OrderCreatedConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration.GetConnectionString("Queue"));
        cfg.ConfigureEndpoints(context);
    });
});

builder.Services.AddHttpClient("orders", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Orders:Url"]!);
});
//.AddPolicyHandler(GetRetryPolicy())
//.AddPolicyHandler(GetCircuitBreakerPolicy());

var app = builder.Build();

await app.Services.GetRequiredService<DatabaseInitializer>().Execute();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapShippingEndpoints();

app.UseHttpsRedirection();

app.Run();

static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
{
    return HttpPolicyExtensions
        .HandleTransientHttpError()
        .WaitAndRetryAsync(3, retryAttempt => 
            TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
            onRetry: (exception, timeSpan, retryCount, context) =>
            {
                Console.WriteLine($"Retry {retryCount} of {context.PolicyKey} at {timeSpan.TotalSeconds} seconds due to: {exception.Exception.Message}");
            });
}

static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
{
    return HttpPolicyExtensions
        .HandleTransientHttpError()
        .CircuitBreakerAsync(5, TimeSpan.FromSeconds(30));
}