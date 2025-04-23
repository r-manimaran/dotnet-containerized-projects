using Serilog;
using Serilog.Sinks.Grafana.Loki;
using Serilog.Templates;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Verbose)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateLogger();

builder.Host.UseSerilog((context, services, loggerConfiguration) => loggerConfiguration
            .ReadFrom.Configuration(context.Configuration)
            .ReadFrom.Services(services)
            .WriteTo.Console(new ExpressionTemplate(
                 "[{@t:HH:mm:ss} {@l:u3}{#if @tr is not null} ({substring(@tr,0,4)}:{substring(@sp,0,4)}){#end}] {@m}\n{@x}"))
             .WriteTo.GrafanaLoki(
                    Environment.GetEnvironmentVariable("loki") ?? "http://loki:3100",
                    new[] { new LokiLabel { Key = "app", Value = "LokiGraf.Api" } },
                    credentials: null,
                    propertiesAsLabels: new[] { "level", "applicationName" }
                ));

builder.Services.AddControllers();

builder.Services.AddOpenApi();

var app = builder.Build();

app.UseSerilogRequestLogging();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
