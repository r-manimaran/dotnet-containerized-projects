
using AspireDemo.RabbitMQConsumer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();
builder.AddRabbitMQClient("messaging");

builder.Services.AddHostedService<OrderProcessingJob>();

var host = builder.Build();
host.Run();
