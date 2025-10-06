using System.Diagnostics;

namespace ECommerceApi.Tracing;

public static class TracingService
{
    public static readonly ActivitySource ActivitySource = new("ECommerceApi.BusinessLogic", "1.0.0");
    
    public static Activity? StartActivity(string name, ActivityKind kind = ActivityKind.Internal)
    {
        return ActivitySource.StartActivity(name, kind);
    }
}