var builder = DistributedApplication.CreateBuilder(args);

var k8s = builder.AddKubernetesEnvironment("k8s-env");

builder.AddDockerComposeEnvironment("env");


var cache = builder.AddRedis("cache");

var seq = builder.AddSeq("seq")
    .WithEnvironment("ACCEPT_EULA", "Y")
    .WithLifetime(ContainerLifetime.Persistent);
   

var apiService = builder.AddProject<Projects.Aspire_DockerCompose_Export_ApiService>("apiservice") 
    .PublishAsAzureAppServiceWebsite((infra, site) =>
    {
        site.SiteConfig.IsWebSocketsEnabled = true;
    })
                        .WithHttpHealthCheck("/health")
                        .WithReference(seq)
                        .WaitFor(seq);


builder.AddProject<Projects.Aspire_DockerCompose_Export_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(cache)
    .WaitFor(cache)
    .WithReference(apiService)
    .WaitFor(apiService)
    .WithReference(seq)
    .WaitFor(seq);


builder.Build().Run();
