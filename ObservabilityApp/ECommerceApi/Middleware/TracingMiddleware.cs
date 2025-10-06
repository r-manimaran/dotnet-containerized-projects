using System.Diagnostics;

namespace ECommerceApi.Middleware;

public class TracingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<TracingMiddleware> _logger;

    public TracingMiddleware(RequestDelegate next, ILogger<TracingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var activity = Activity.Current;
        
        // Add custom tags to the current span
        activity?.SetTag("http.user_agent", context.Request.Headers.UserAgent.ToString());
        activity?.SetTag("http.client_ip", context.Connection.RemoteIpAddress?.ToString());
        activity?.SetTag("http.request_id", context.TraceIdentifier);
        
        // Add correlation ID
        var correlationId = context.Request.Headers["X-Correlation-ID"].FirstOrDefault() 
                           ?? Guid.NewGuid().ToString();
        activity?.SetTag("correlation.id", correlationId);
        context.Response.Headers.Append("X-Correlation-ID", correlationId);

        // Track request size
        if (context.Request.ContentLength.HasValue)
        {
            activity?.SetTag("http.request_content_length", context.Request.ContentLength.Value);
        }

        var stopwatch = Stopwatch.StartNew();
        
        try
        {
            await _next(context);
            
            // Add response information
            activity?.SetTag("http.response_content_length", context.Response.ContentLength ?? 0);
            activity?.SetTag("http.response_time_ms", stopwatch.Elapsed.TotalMilliseconds);
            
            // Mark slow requests
            if (stopwatch.Elapsed.TotalMilliseconds > 2000)
            {
                activity?.SetTag("performance.slow_request", true);
                _logger.LogWarning("Slow request detected: {Method} {Path} took {Duration}ms", 
                    context.Request.Method, context.Request.Path, stopwatch.Elapsed.TotalMilliseconds);
            }
        }
        catch (Exception ex)
        {
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            activity?.SetTag("error.type", ex.GetType().Name);
            activity?.SetTag("error.message", ex.Message);
            throw;
        }
    }
}