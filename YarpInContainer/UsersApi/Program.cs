using Microsoft.EntityFrameworkCore;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using UsersApi.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHeaderPropagation(opt => opt.Headers.Add("azure-correlation-id"));

builder.Host.UseSerilog((context, configuration) => configuration
            .ReadFrom.Configuration(context.Configuration));

builder.Services.AddDbContext<UserDbContext>(option =>
    option.UseInMemoryDatabase("users"));

builder.Services.AddControllers();

builder.Services.AddOpenApi();

builder.Services.AddOpenTelemetry()
       .ConfigureResource(r => r.AddService("usersApi"))
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

app.UseAuthorization();

app.MapControllers();

app.Run();
