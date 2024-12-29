var builder = DistributedApplication.CreateBuilder(args);

// Adding Resources
var postgres = builder.AddPostgres("postgres")
                      .WithDataVolume()
                      .AddDatabase("url-shortner");

var redis = builder.AddRedis("redis");

builder.AddProject<Projects.UrlShortner_Api>("urlshortner-api")
        .WithReference(postgres)
        .WithReference(redis)
        .WaitFor(postgres)
        .WaitFor(redis);

builder.Build().Run();
