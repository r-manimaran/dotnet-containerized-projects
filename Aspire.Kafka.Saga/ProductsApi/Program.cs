using Confluent.Kafka;
using eCommerce.Common;
using Microsoft.EntityFrameworkCore;
using ProductsApi.Data;
using ProductsApi.Endpoints;
using ProductsApi.Kafka;
using SharedLib.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddOpenApi();

builder.AddKafkaProducer<string, string>("messaging", static settings => settings.DisableHealthChecks = true);

builder.AddKafkaConsumer<string, string>("messaging", options =>
{
    options.Config.GroupId = "order-created";
    options.Config.AutoOffsetReset = AutoOffsetReset.Earliest;    
});

builder.Services.AddSingleton<IKafkaProducer, KafkaProducer>();
builder.Services.AddHostedService<ProductConsumer>();

builder.Services.AddDbContext<ProductDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("postgres"));
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

var app = builder.Build();

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.MapEndpointExplorers();
}

app.UseCors("AllowAll");
app.MapProductEndpoints();

app.UseHttpsRedirection();

app.Run();

