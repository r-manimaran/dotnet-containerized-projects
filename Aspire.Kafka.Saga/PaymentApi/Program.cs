using Confluent.Kafka;
using eCommerce.Common;
using PaymentApi.Kafka;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddOpenApi();

builder.AddKafkaProducer<string, string>("messaging", 
    static settings => settings.DisableHealthChecks = true);

builder.AddKafkaConsumer<string, string>("messaging", options =>
{
    options.Config.GroupId = "order-created";
    options.Config.AutoOffsetReset = AutoOffsetReset.Earliest;
});

builder.Services.AddSingleton<IKafkaProducer, KafkaProducer>();

builder.Services.AddHostedService<PaymentConsumer>();

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.Run();

