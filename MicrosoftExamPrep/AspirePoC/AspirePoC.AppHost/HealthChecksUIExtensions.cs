using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Aspire.Hosting.Lifecycle;
namespace AspirePoC.AppHost;

public static class HealthChecksUIExtensions
{
    public static IResourceBuilder<HealthChecksUIResource> AddHealthChecksUI(this IDistributedApplicationBuilder builder,
        string name, int? port=null)
    {
        builder.Services.TryAddLifecycleHook<HealthCheckUILifecycleHook>();

        var resource = new HealthChecksUIResource(name);

        return builder
            .AddResource(resource)
            .WithImage(HealthChecksUIDefaults.ContainerImageName, HealthChecksUIDefaults.ContainerImageTag)
            .WithImageRegistry(HealthChecksUIDefaults.ContainerRegistry)
            .WithEnvironment(HealthChecksUIResource.KnownEnvVars.UiPath, "/")
            .WithHttpEndpoint(port: port, targetPort: HealthChecksUIDefaults.ContainerPort);
    }
}
