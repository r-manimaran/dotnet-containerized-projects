using Carter;
using UrlShortner.Api.Services;
var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add reference to .Net Aspire Host
builder.AddNpgsqlDataSource("url-shortner");
builder.AddRedisDistributedCache("redis");

#pragma warning disable EXTEXP0018 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
builder.Services.AddHybridCache();
#pragma warning restore EXTEXP0018 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

builder.Services.AddOpenApi();
builder.Services.AddCarter();

builder.Services.AddHostedService<DatabaseInitializer>();
builder.Services.AddScoped<IUrlShortnerService, UrlShortnerService>();
builder.Services.AddHttpContextAccessor();

builder.Services.AddOpenTelemetry()
        .WithMetrics(metrics=>metrics.AddMeter("UrlShortening.Api"));

var app = builder.Build();

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "OpenAPI V1");
    });
}

app.UseHttpsRedirection();

// Map the endpoints
app.MapCarter();

app.Run();


