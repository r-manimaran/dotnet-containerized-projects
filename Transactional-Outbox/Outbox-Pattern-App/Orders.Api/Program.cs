using MassTransit;
using MassTransit.Transports.Fabric;
using Message.Contracts;
using Microsoft.EntityFrameworkCore;
using Orders.Api;
using Orders.Api.Endpoints;
using Orders.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddLogging();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});


builder.Services.AddScoped<IOrderService, OrderService>();

builder.Services.AddHostedService<OutboxBackgroundService>();

builder.Services.AddScoped<OutboxProcessor>();

// Add MassTransit for RabbitMQ to Publish the message
builder.Services.AddMassTransit(config =>
{
    config.UsingRabbitMq((context, cfg) =>
    {
        // cfg.Host(builder.Configuration.GetConnectionString("Queue"));
        
        cfg.Host("localhost", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        // configure the publish endpoint
        cfg.Message<OrderPublish>(x=> {
            x.SetEntityName("order-publish");
        });

        cfg.ConfigureEndpoints(context);

        //cfg.Publish<OrderPublish>(x=> {
        //    x.ExchangeType = ExchangeType.Direct;
        //});
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapOrderEndpoints();

app.Run();

