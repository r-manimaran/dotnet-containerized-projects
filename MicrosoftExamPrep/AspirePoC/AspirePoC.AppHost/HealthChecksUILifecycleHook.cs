using Aspire.Hosting.Lifecycle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspirePoC.AppHost;

internal class HealthChecksUILifecycleHook(DistributedApplicationExecutionContext executionContext): IDistributedApplicationLifecycleHook
{
    private const string HEALTHCHECKSUI_URLS = "HEALTHCHECSUI_URLS";
    public Task BeforeStartAsync(DistributedApplicationModel appModel, CancellationToken cancellationToken=default)
    {
        return Task.CompletedTask;
    }

    public Task AfterEndpointsAllocatedAsync(DistributedApplicationModel appModel, CancellationToken cancellationToken=default)
    {
        return Task.CompletedTask;
    }

    private static void ConfigureHealthChecksUIContainers(IResourceCollection resources, bool isPublishing)
    {

    }
}

