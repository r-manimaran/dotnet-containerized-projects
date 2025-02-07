using Microsoft.EntityFrameworkCore;
using OrdersApi.Data;
using OrdersApi.Endpoints;
using OrdersApi.Kafka;
using SharedLib.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddOpenApi();

builder.Services.AddDbContext<OrderDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("sqlserver"));
});
builder.AddKafkaProducer<string, string>("messaging");
builder.Services.AddScoped<IKafkaProducer, KafkaProducer>();

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

    // Map EndpointsExplorer
    app.MapEndpointExplorers();
}
app.UseCors("AllowAll");

app.MapOrdersEndpoints();

app.UseHttpsRedirection();

app.Run();


