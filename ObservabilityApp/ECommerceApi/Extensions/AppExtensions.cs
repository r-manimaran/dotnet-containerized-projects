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
                       .ConfigureResource(r => r.AddService("ECommerceApi"))
                       .WithTracing(tracing =>
                            tracing.
                                    AddHttpClientInstrumentation(opt => {
                                        opt.RecordException = true;
                                    })
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
                                   .AddOtlpExporter(opt => opt.Endpoint = new Uri(jaegerEndpoint)))
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
