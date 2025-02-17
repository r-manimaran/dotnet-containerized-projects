using Aspire.Database.TestContainers.AppHost.Extensions;

var builder = DistributedApplication.CreateBuilder(args);

var redis = builder.AddRedis("redis");
var postgres = builder.AddPostgres("postgres")
                      .WithPgAdmin()
                      .WithDataVolume()
                      .AddDatabase("products");


var seq = builder.AddSeq("seq");
                       

var apiService = builder.AddProject<Projects.Aspire_Database_TestContainers>("apiService")
                    .WithReference(postgres)
                    .WaitFor(postgres)
                    .WithReference(redis)
                    .WaitFor(redis)
                    .WithReference(seq)
                    .WaitFor(seq);

apiService.WithSwaggerUi()
          .WithRedocUi();

builder.Build().Run();
