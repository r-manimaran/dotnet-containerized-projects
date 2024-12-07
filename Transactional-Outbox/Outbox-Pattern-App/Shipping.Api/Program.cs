using MassTransit;
using Message.Contracts;
using Shipping.Api;
using Shipping.Api.Endpoints;
using Shipping.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Connect to Sqlite DB
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddSqlite<AppDbContext>(connectionString);

builder.Services.AddScoped<IShippingService , ShippingService>();

//configure MassTransit for RabbitMQ
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<OrderCreatedEventConsumer>();
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        cfg.ReceiveEndpoint("order-publish",e=> {
            e.ConfigureConsumer<OrderCreatedEventConsumer>(context, c=> {
                 c.UseMessageRetry(r => r.Intervals(100, 200, 500, 1000));
                c.UseRateLimit(1000, TimeSpan.FromMinutes(1));
            });
        });

        // Add retry policy
        cfg.UseMessageRetry(r => 
        {
            r.Incremental(3, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2));
        });
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

// Map Shipment Endpoints
app.MapShipmentEndpoints();

app.Run();


