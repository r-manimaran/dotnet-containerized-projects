using MassTransit;
using Microsoft.EntityFrameworkCore;
using Products.Api;
using Products.Api.Repositories;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<ProductDbContext>(opt =>
{
    opt.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddScoped<IProduct,ProductRepository>();

builder.Services.AddMassTransit(x =>
{
   x.UsingRabbitMq((context, config) =>
    {
        config.Host("rabbitmq://localhost", c =>
        {
            c.Username("guest");
            c.Password("guest");
        });       
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
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
