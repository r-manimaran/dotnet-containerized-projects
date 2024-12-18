var builder = DistributedApplication.CreateBuilder(args);

var redis = builder.AddRedis("redis");

builder.AddProject<Projects.Aspire_Redis_HybridCache>("aspire-redis-hybridcache")
    .WithReference(redis)
    .WaitFor(redis);

builder.Build().Run();
