using Confluent.Kafka;
using Orders.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddTransient<IOrderService, OrderService>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure Kafka producer settings
var kafkaProducerConfig = new ProducerConfig
{
    BootstrapServers = "localhost:9092",
    //ClientId = "producer-client"
};
builder.Services.AddSingleton<IProducer<Null,string>>(x=> 
     new ProducerBuilder<Null, string>(kafkaProducerConfig).Build());

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
