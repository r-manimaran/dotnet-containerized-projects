using System.Diagnostics;

namespace ECommerceApi;

public class ApiMetricsMiddleware
{
    private readonly RequestDelegate _next;

    public ApiMetricsMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        var endpoint = context.GetEndpoint()?.DisplayName ?? "unknown";
        try
        {
            await _next(context);
            var statusCode = context.Response.StatusCode.ToString();
            MetricsService.ApiRequestCounter
                .WithLabels(endpoint, context.Request.Method, statusCode)
                .Inc();
        }
        catch(Exception ex)
        {
            MetricsService.ErrorCounter
                .WithLabels(endpoint, ex.GetType().Name)
                .Inc();
            throw;
        }
        finally
        {
            stopwatch.Stop();
            MetricsService.ApiRequestDuration
                .WithLabels(endpoint, context.Request.Method)
                .Observe(stopwatch.Elapsed.TotalSeconds);
        }
    }
}
