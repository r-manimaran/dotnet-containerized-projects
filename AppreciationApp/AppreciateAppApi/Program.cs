using AppreciateAppApi.Data;
using AppreciateAppApi.Endpoints;
using AppreciateAppApi.Extensions;
using AppreciateAppApi.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Scalar.AspNetCore;
using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();
builder.Services.AddOpenApi();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("Appreciation"));
    // Remove the UseSnakeCaseNamingConvention line as it is not a valid method
    //options.UseSnakeCaseNamingConvention();
});

builder.Services.AddScoped<IAppreciationService, AppreciationService>();

builder.Services.AddScoped<IEmployeeService, EmployeeService>();

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

// Configure Swagger/OpenAPI
app.MapOpenApi();

app.MapScalarApiReference();

app.UseSwagger();

app.UseSwaggerUI();

app.MapDefaultEndpoints();

app.UseDeveloperExceptionPage();

app.UseHttpsRedirection();

app.MapAppreciationEndpoints();

app.MapEmployeesEndpoints();

await app.ApplyMigration();



await app.RunAsync();


