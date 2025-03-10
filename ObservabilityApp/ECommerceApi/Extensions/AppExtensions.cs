using Microsoft.EntityFrameworkCore;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace ECommerceApi.Extensions;

public static class AppExtensions
{
    public static void ConfigureOpenTelemetry(this IHostApplicationBuilder builder)
    {
         var jaegerEndpoint = builder.Configuration["Jaeger:Endpoint"] 
        ?? "http://api.jaeger:4317";

        builder.Logging.AddOpenTelemetry(logging =>
        {
            logging.IncludeFormattedMessage = true;
            logging.IncludeScopes = true;
        });

        builder.Services.AddOpenTelemetry()
                       .ConfigureResource(r => r.AddService("ECommerceApi")
                       .AddEnvironmentVariableDetector()
                       .AddTelemetrySdk())
                       .WithTracing(tracing =>
                            tracing
                                    // Set sampling to 100%
                                    .SetSampler(new AlwaysOnSampler())
                                   // To Track outgoing HTTP requests made via HttpClient
                                    .AddHttpClientInstrumentation(opt => {
                                        opt.RecordException = true;
                                    }).SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("CustomerApiService"))
                                   // To Capture Incoming requests to the API
                                   .AddAspNetCoreInstrumentation(opts => {
                                       opts.RecordException = true;
                                       opts.EnrichWithHttpRequest = (activity, httpRequest) => {
                                           activity.SetTag("requestProtocol", httpRequest.Protocol);
                                       };
                                   })
                                   .AddEntityFrameworkCoreInstrumentation(opts =>
                                   {
                                       opts.SetDbStatementForStoredProcedure = true;
                                       opts.SetDbStatementForText = true;
                                   })
                                   .AddSqlClientInstrumentation(options =>
                                    {
                                        options.SetDbStatementForText = true;
                                        options.RecordException = true;
                                    })
                                   .AddSource("EShop.Api")
                                   .AddOtlpExporter(opt => 
                                   {
                                    opt.Endpoint = new Uri(jaegerEndpoint);
                                    opt.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.Grpc;
                                    // Add timeout settings
                                    opt.TimeoutMilliseconds = 15000;
                                    opt.ExportProcessorType = ExportProcessorType.Batch;
                                    })) // Send the trace data to an OTLP collector
                                   
                       .WithMetrics(metrics =>
                            metrics.AddAspNetCoreInstrumentation()
                                   .AddHttpClientInstrumentation()
                                   .AddRuntimeInstrumentation()
                                   .AddMeter("MyApi.CustomersService")
                                   .AddPrometheusExporter());
                       //.UseOtlpExporter();    
    }

    public static void AddDatabase(this IHostApplicationBuilder builder)
    {
        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));
    }

    public static async Task ApplyMigrations(this IApplicationBuilder app)
    {
        using (var scope = app.ApplicationServices.CreateScope())
        {
            
            await using (var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>())
            {
                await dbContext.Database.MigrateAsync();
                //await DatabaseSeedService.SeedAsync(dbContext);
            }
        }
    }
}
