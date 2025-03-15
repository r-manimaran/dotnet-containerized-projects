namespace AspirePoC.AppHost;

public static class HealthChecksUIDefaults
{
    public const string ContainerRegistry = "docker.io";

    public const string ContainerImageName = "xabarilcoding/healthchecksui";

    public const string ContainerImageTag = "5.0.0";

    public const int ContainerPort = 80;

    public const string ProbePath = "/healthz";

    public const string EndpointName = "healthchecks";
}
