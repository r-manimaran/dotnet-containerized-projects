using Hangfire.Annotations;
using Hangfire.Dashboard;

namespace LongRunningApi;

public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize([NotNull] DashboardContext context)
    {
        // For development/testing - allows all access
        // In production, implement proper authorization logic
        var httpContext = context.GetHttpContext();

        // Example: Allow only authenticated users
        //return httpContext.User.Identity?.IsAuthenticated ?? false;

        // Example: Check for specific role
        // return httpContext.User.IsInRole("AdminRole");
        return true;
    }
}
