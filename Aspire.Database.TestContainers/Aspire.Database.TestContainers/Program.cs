using Aspire.Database.TestContainers;
using Aspire.Database.TestContainers.Data;
using Aspire.Database.TestContainers.Extensions;
using Aspire.Database.TestContainers.Services;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddHeaderPropagation(options => options.Headers.Add("my-custom-correlation-id"));
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()    
    .CreateLogger();

builder.Host.UseSerilog((context, loggerConfig)=>
{
    loggerConfig.Enrich.FromLogContext()
    .ReadFrom.Configuration(context.Configuration)
    .Enrich.WithCorrelationIdHeader("my-custom-correlation-id");
});

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("products"));
});

builder.AddRabbitMQClient("messaging");

builder.Services.AddHostedService<TodoConsumer>();

builder.Services.AddScoped<IProductService , ProductService>();

builder.Services.AddControllers();

builder.Services.AddOpenApi();

var app = builder.Build();

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwaggerUI(options => {
        options.SwaggerEndpoint(
        "/openapi/v1.json", "OpenAPI v1");
    });

    app.UseReDoc(options => {
        options.SpecUrl("/openapi/v1.json");
    });

    app.ApplyMigrations();
}

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
