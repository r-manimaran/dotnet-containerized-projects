using Shipment.api.Services;
using Confluent.Kafka;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton<IShippingService,ShippingService>();
builder.Services.AddControllers();


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Configure the Kafka Consumer
var kafkaConfig = new ConsumerConfig
{
    GroupId = "new-order-group",
    BootstrapServers = "localhost:9092",
    AutoOffsetReset = AutoOffsetReset.Earliest
};

builder.Services.AddSingleton<IConsumer<Null,string>>(x=> 
    new ConsumerBuilder<Null,string>(kafkaConfig).Build());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
