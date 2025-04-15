using Microsoft.EntityFrameworkCore;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using ProductsApi.Data;
using Serilog;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddOpenApi();

builder.Services.AddHeaderPropagation(opt => opt.Headers.Add("azure-correlation-id"));

builder.Host.UseSerilog((context, configuration) => configuration
            .ReadFrom.Configuration(context.Configuration));

builder.Services.AddDbContext<ProductDbContext>(option =>
    option.UseInMemoryDatabase("products"));

// Add Health Checks
builder.Services.AddHealthChecks()
      .AddCheck("self", () => HealthCheckResult.Healthy("Products API is running."));


builder.Services.AddOpenTelemetry()
       .ConfigureResource(r => r.AddService("productApi"))
       .WithTracing(tracing =>
            tracing.
                    AddHttpClientInstrumentation()
                   .AddAspNetCoreInstrumentation()
                   .AddEntityFrameworkCoreInstrumentation())
       .UseOtlpExporter();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseSwaggerUI(opt =>
    opt.SwaggerEndpoint("/openapi/v1.json", "OpenAPI v1"));

//app.UseHttpsRedirection();

app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";
        
        var response = new
        {
            Status = report.Status.ToString(),
            Duration = report.TotalDuration,
            Info = report.Entries.Select(e => new
            {
                Key = e.Key,
                Status = e.Value.Status.ToString(),
                Description = e.Value.Description,
                Duration = e.Value.Duration
            })
        };
        
        await context.Response.WriteAsJsonAsync(response);
    }
});


app.UseAuthorization();

app.MapControllers();

app.Run();
