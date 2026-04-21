var builder = DistributedApplication.CreateBuilder(args);
var redis = builder.AddRedis("cache", port: 6379)
                    .WithRedisInsight()
                    .WithLifetime(ContainerLifetime.Persistent);

builder.AddProject<Projects.WebApi>("webapi")
    .WithHttpsEndpoint(5001,name:"extra-https")
    .WithReference(redis).WaitFor(redis)
    .WithReplicas(2);

builder.Build().Run();
