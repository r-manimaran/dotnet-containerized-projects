var builder = DistributedApplication.CreateBuilder(args);

// postgres Database
var postgres = builder.AddPostgres("postgres")
                      .WithPgAdmin()
                      .WithLifetime(ContainerLifetime.Persistent);

if(builder.ExecutionContext.IsRunMode)
{
    // Data volumes don't work on ACA for postgres so only add when running
    postgres.WithDataVolume();
}
var catalogDb = postgres.AddDatabase("catalogdb");

// Redis cache
var basketCache = builder.AddRedis("basketCache")
                         .WithDataVolume()
                         .WithRedisCommander();

builder.AddProject<Projects.Aspire_eshop_CatelogDbManager>("catelogdbmanager");

builder.AddProject<Projects.Aspire_eshop_WebApp>("Blazar-webapp");

builder.AddProject<Projects.Aspire_eshop_BasketService>("basket-service");

builder.AddProject<Projects.Aspire_eshop_CatalogService>("catalog-service");

builder.Build().Run();
