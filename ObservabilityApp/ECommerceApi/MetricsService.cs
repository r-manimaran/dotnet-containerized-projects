using Prometheus;
using System.Diagnostics.Metrics;

namespace ECommerceApi;

public static class MetricsService
{
    // API Request/Response Metrics
    public static readonly Counter ApiRequestCounter = Metrics
        .CreateCounter("api_requests_total", "Total API Requests",
        new CounterConfiguration
        {
            LabelNames = new[] { "endpoint", "method", "status" }
        });

    public static readonly Histogram ApiRequestDuration = Metrics
        .CreateHistogram("api_request_duration_seconds", "API Request Duration",
            new HistogramConfiguration
            {
                LabelNames = new[] { "endpoint", "method" },
                Buckets = Histogram.LinearBuckets(start: 0.1, width: 0.2, count: 5)
            });

    // Error Tracking
    public static readonly Counter ErrorCounter = Metrics
        .CreateCounter("api_errors_total", "Total API Errors", new CounterConfiguration
        {
            LabelNames = new[] { "endpoint", "exception_type" }
        });

    // Dependency Calls
    public static readonly Counter DependencyCallCounter = Metrics
        .CreateCounter("dependency_calls_total", "External Dependency Calls", new CounterConfiguration
        {
            LabelNames = new[] { "service", "success" }
        });

    public static readonly Histogram DependencyCallDuration = Metrics
        .CreateHistogram("dependency_call_duration_seconds", "Dependency Call Duration",
            new HistogramConfiguration
            {
                LabelNames = new[] { "service" },
                Buckets = Histogram.ExponentialBuckets(0.01, 2, 10)
            });

    // Cache Metrics
    public static readonly Counter CacheCounter = Metrics
        .CreateCounter("cache_operations_total", "Cache Operations", new CounterConfiguration
        {
            LabelNames = new[] { "operation", "result" }
        });
}
