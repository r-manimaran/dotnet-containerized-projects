using Carter;
using Orders.Api;
using Orders.Api.Extensions;
using Scalar.AspNetCore;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Steeltoe.Discovery.Client;
using Steeltoe.Discovery.Consul;
using Steeltoe.Common.Http.Discovery;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.RegisterServices();

//client.BaseAddress = new Uri("http://reporting.api:80");
builder.Services.AddHttpClient<Client>(client =>
{
    client.BaseAddress = new Uri("http://reporting-service");   
}).AddServiceDiscovery()
.AddRoundRobinLoadBalancer();

builder.Services.AddHealthChecks();
builder.Services
    .AddOpenTelemetry()
    .ConfigureResource(resource=> resource.AddService("Orders.Api"))
    .WithTracing(tracing =>
    {
        tracing
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()        
        .AddSource("Orders.Api");

        tracing.AddOtlpExporter(opt => opt.Endpoint = new Uri("http://api.jaeger:4317"));
        tracing.AddConsoleExporter();
    });
builder.Services.AddServiceDiscovery(o => o.UseConsul());
var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();    
}

app.UseSwagger();
app.UseSwaggerUI();
app.MapScalarApiReference();

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");
app.MapHealthChecks("/health");
app.MapCarter();
app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
