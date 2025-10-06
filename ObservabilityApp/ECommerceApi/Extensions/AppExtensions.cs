using Microsoft.EntityFrameworkCore;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using ECommerceApi.Tracing;

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
                                    .SetSampler(new AlwaysOnSampler())
                                    .AddHttpClientInstrumentation(opt => {
                                        opt.RecordException = true;
                                        opt.EnrichWithHttpRequestMessage = (activity, httpRequestMessage) => {
                                            activity.SetTag("http.request.header.user_agent", 
                                                httpRequestMessage.Headers.UserAgent?.ToString());
                                        };
                                        opt.EnrichWithHttpResponseMessage = (activity, httpResponseMessage) => {
                                            activity.SetTag("http.response.status_text", httpResponseMessage.ReasonPhrase);
                                        };
                                    })
                                   .AddAspNetCoreInstrumentation(opts => {
                                       opts.RecordException = true;
                                       opts.EnrichWithHttpRequest = (activity, httpRequest) => {
                                           activity.SetTag("http.request.protocol", httpRequest.Protocol);
                                           activity.SetTag("http.request.content_type", httpRequest.ContentType);
                                           activity.SetTag("http.request.query_string", httpRequest.QueryString.ToString());
                                       };
                                       opts.EnrichWithHttpResponse = (activity, httpResponse) => {
                                           activity.SetTag("http.response.content_type", httpResponse.ContentType);
                                       };
                                   })
                                   .AddEntityFrameworkCoreInstrumentation(opts =>
                                   {
                                       opts.SetDbStatementForStoredProcedure = true;
                                       opts.SetDbStatementForText = true;
                                       opts.EnrichWithIDbCommand = (activity, command) => {
                                           activity.SetTag("db.command.timeout", command.CommandTimeout);
                                           activity.SetTag("db.command.type", command.CommandType.ToString());
                                       };
                                   })
                                   .AddSqlClientInstrumentation(options =>
                                    {
                                        options.SetDbStatementForText = true;
                                        options.RecordException = true;
                                        //options.EnableConnectionLevelAttributes = true;
                                    })
                                   .AddSource("EShop.Api")
                                   .AddSource("ECommerceApi.BusinessLogic")
                                   .AddOtlpExporter(opt => 
                                   {
                                    opt.Endpoint = new Uri(jaegerEndpoint);
                                    opt.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.Grpc;
                                    opt.TimeoutMilliseconds = 15000;
                                    opt.ExportProcessorType = ExportProcessorType.Batch;
                                    }))
                       .WithMetrics(metrics =>
                            metrics.AddAspNetCoreInstrumentation()
                                   .AddHttpClientInstrumentation()
                                   .AddRuntimeInstrumentation()
                                   .AddMeter("MyApi.CustomersService")
                                   .AddPrometheusExporter());
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
