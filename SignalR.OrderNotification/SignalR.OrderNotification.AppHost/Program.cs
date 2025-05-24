var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache")
                   .WithRedisInsight()
                   .WithLifetime(ContainerLifetime.Persistent);

builder.AddProject<Projects.OrderApiService>("orderapiservice")
       .WithReference(cache)
       .WaitFor(cache);

builder.Build().Run();
