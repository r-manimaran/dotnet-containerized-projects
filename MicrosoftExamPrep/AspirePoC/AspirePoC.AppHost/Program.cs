using Microsoft.IdentityModel.Tokens;

var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache")
                   .ExcludeFromManifest();
    

var seq = builder.AddSeq("seq")
                 .ExcludeFromManifest()
                 .WithLifetime(ContainerLifetime.Persistent)
                 .WithEnvironment("ACCEPT_EULA", "Y");

var apiService = builder.AddProject<Projects.AspirePoC_ApiService>("apiservice")
    .WithReference(seq)
    .WaitFor(seq)
    .WithReference(cache)
    .WaitFor(cache);

builder.AddContainer("hello", "mcr.microsoft.com/azuredocs/aci-helloworld")
       .WithEnvironment("PoC", "APL")
       .WithEnvironment("ASPNETCORE_URLS", "http://+:0");
        //.WithHttpEndpoint(port: 0, name: "http");

builder.AddProject<Projects.AspirePoC_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(apiService);



builder.Build().Run();
