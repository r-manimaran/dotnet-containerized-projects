using MassTransit;
using Microsoft.EntityFrameworkCore;
using Orders.Api;
using Orders.Api.Consumer;
using Orders.Api.Repositories;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddOpenApi();

builder.Services.AddDbContext<OrderDbContext>(opt =>
{
    opt.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddScoped<IOrder, OrderRepository>();
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<ProductConsumer>();
    x.UsingRabbitMq((context, config) =>
    {
        config.Host("rabbitmq://localhost", c =>
        {
            c.Username("guest");
            c.Password("guest");
        });
        config.ReceiveEndpoint("product-queue", e =>
        {
            e.ConfigureConsumer<ProductConsumer>(context);
        });
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapScalarApiReference(opt =>
{
    opt.Theme = ScalarTheme.Kepler;
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
