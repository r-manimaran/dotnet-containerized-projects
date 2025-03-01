using Microsoft.EntityFrameworkCore;
using ProductsApi.Data;
using ProductsApi.Services;
using Npgsql.EntityFrameworkCore.PostgreSQL;
using Npgsql;
using System.Diagnostics; // Add this using directive for PostgreSQL extensions

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddControllers();

builder.Services.AddOpenApi();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("productsdb"));
    // Remove the UseSnakeCaseNamingConvention line as it is not a valid method
    //options.UseSnakeCaseNamingConvention();
});

builder.Services.AddHostedService<DatabaseSeeder>();
builder.Services.AddOpenTelemetry()
    .WithTracing(tracerProviderBuilder =>
    {
        tracerProviderBuilder
            .AddSource("Npgsql") // Capture Npgsql activities
            .AddSource("Npgsql.Command") // Capture SQL statements
            .AddNpgsql(); // Npgsql-specific instrumentation
            
        }
    );

System.Diagnostics.ActivitySource.AddActivityListener(
    new ActivityListener
    {
        ShouldListenTo = source => source.Name.StartsWith("Npgsql"),
        ActivityStarted = _ => { },
        ActivityStopped = _ => { }
    });

var app = builder.Build();

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwaggerUI(options => {
        options.SwaggerEndpoint(
        "/openapi/v1.json", "OpenAPI v1");
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
