using Hangfire;
using Hangfire.PostgreSql;
using Npgsql;
using OpenTelemetry.Resources;
using OpenTelemetry;
using OpenTelemetry.Trace;
using Microsoft.AspNetCore.SignalR;
using LongRunningApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddCors();

builder.Services.AddSignalR();

builder.Services.AddHangfire(
    config => config.UsePostgreSqlStorage(
        options => options.UseNpgsqlConnection(builder.Configuration.GetConnectionString("Database"))));

builder.Services.AddHangfireServer(options => options.SchedulePollingInterval = TimeSpan.FromSeconds(1));

builder.Services.AddTransient<LongRunningJob>();

builder.Services.AddTransient<LongRunningJobWithNotification>();

builder.Services.AddOpenTelemetry()
       .ConfigureResource(r => r.AddService("longRunningApi"))
       .WithTracing(tracing =>
            tracing.
                   AddNpgsql()
                   .AddHttpClientInstrumentation()
                   .AddAspNetCoreInstrumentation()
                   .AddEntityFrameworkCoreInstrumentation())
       .UseOtlpExporter();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(opt =>
        opt.SwaggerEndpoint("/openapi/v1.json", "openapi v1"));
}

app.UseHttpsRedirection();

app.UseCors(o=>o.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin().WithExposedHeaders("*"));

app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization = new[] { new HangfireAuthorizationFilter() }
});

app.MapGet("reports/v1", async (ILogger<Program> logger) =>
{
    logger.LogInformation("tarting background job");

    await Task.Delay(TimeSpan.FromSeconds(10));

    logger.LogInformation("Completed background job");

    return "Completed";
});

app.MapPost("reports/v2", (IBackgroundJobClient backgroundJobClient) =>
{
    string jobId = backgroundJobClient.Enqueue<LongRunningJob>(job => job.ExecuteAsync(CancellationToken.None));

    return Results.AcceptedAtRoute("JobDetails", new { jobId }, jobId);
});


app.MapPost("reports/v3", async (IBackgroundJobClient backgroundJobClient, IHubContext<NotificationHub> hubContext) =>
{
    
    string jobId = backgroundJobClient.Enqueue<LongRunningJobWithNotification>(job => job.ExecuteAsync(CancellationToken.None));

    await hubContext.Clients.All.SendAsync("ReceiveNotification", $"Started processing job with Id:{jobId}");
    
    return Results.AcceptedAtRoute("JobDetails", new { jobId }, jobId);
});

app.MapGet("jobs/{jobId}", (string jobId) =>
{
    var jobDetails = JobStorage.Current.GetMonitoringApi().JobDetails(jobId);
    
    return jobDetails.History.OrderByDescending(h => h.CreatedAt).First().StateName;

}).WithName("JobDetails");

app.MapHub<NotificationHub>("notifications");

app.Run();

public class NotificationHub: Hub;
public class LongRunningJob(ILogger<LongRunningJob> logger)
{
    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("starting background job");

        await Task.Delay(TimeSpan.FromSeconds(10));

        logger.LogInformation("Completed background job");
    }
}

public class LongRunningJobWithNotification(ILogger<LongRunningJob> logger, IHubContext<NotificationHub> hubContext)
{
    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("starting background job");

        await Task.Delay(TimeSpan.FromSeconds(10));

        logger.LogInformation("Completed background job");

        await hubContext.Clients.All.SendAsync("ReceiveNotification", "Completed processing job");
    }
}
