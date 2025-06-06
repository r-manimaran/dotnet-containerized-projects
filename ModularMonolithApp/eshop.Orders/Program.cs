using eshop.Orders;
using eshop.Orders.Modules.Orders;
using eshop.Orders.Modules.Orders.PublicApi;
using eshop.Orders.Modules.Shipping;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Npgsql;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
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

builder.Services.AddDbContext<OrderDbContext>(options =>
    options.EnableSensitiveDataLogging()
           .UseNpgsql(builder.Configuration.GetConnectionString("Database"),
            x => x.MigrationsHistoryTable(HistoryRepository.DefaultTableName, "orders"))
          .UseSnakeCaseNamingConvention());


builder.Services.AddDbContext<ShippingDbContext>(options =>
    options.EnableSensitiveDataLogging()
            .UseNpgsql(builder.Configuration.GetConnectionString("Database"),
             x => x.MigrationsHistoryTable(HistoryRepository.DefaultTableName, "shipping"))
            .UseSnakeCaseNamingConvention());

builder.Services.AddScoped<IOrderService, OrderService>();

builder.Services.AddOpenTelemetry()
       .ConfigureResource(r => r.AddService("eshop.Orders"))
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

    x.UsingInMemory((context, cfg) =>
    {
        cfg.ConfigureEndpoints(context);
    });
});
var app = builder.Build();

await app.Services.GetRequiredService<DatabaseInitializer>().Execute();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapOrdersEndpoints();
//app.MapShippingEndpoints();

app.UseHttpsRedirection();

app.Run();

