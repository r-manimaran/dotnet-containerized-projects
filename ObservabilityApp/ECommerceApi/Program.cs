using ECommerceApi;
using ECommerceApi.Extensions;
using ECommerceApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddOpenApi();

builder.ConfigureOpenTelemetry();

builder.AddDatabase();

builder.Services.AddScoped<ICustomerService, CustomerService>();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
     options.SwaggerEndpoint("/openapi/v1.json", "OpenApi v1"));
}

app.MapFallbackToFile("index.html");

app.MapPrometheusScrapingEndpoint();

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseMiddleware<ApiMetricsMiddleware>();

app.MapControllers();

app.ApplyMigrations().Wait() ;

app.Run();
