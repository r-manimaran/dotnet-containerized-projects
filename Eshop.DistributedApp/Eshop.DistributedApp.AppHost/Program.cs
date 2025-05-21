using Eshop.DistributedApp.AppHost;

var builder = DistributedApplication.CreateBuilder(args);

// DB Service
var postgres = builder.AddPostgres("postgres")
                      .WithPgAdmin()
                      .WithLifetime(ContainerLifetime.Persistent);

var catalogDb = postgres.AddDatabase("catalogdb");

// cache Service
var cache = builder.AddRedis("cache")
                   .WithRedisInsight()
                   .WithLifetime(ContainerLifetime.Persistent);

// message service
var rabbitmq = builder.AddRabbitMQ("rabbitmq")
                      .WithManagementPlugin()
                      .WithLifetime(ContainerLifetime.Persistent);

// Key cloak Service
var keycloak = builder.AddKeycloak("keycloak", 8080)
                      .WithLifetime(ContainerLifetime.Persistent);

if(builder.ExecutionContext.IsRunMode)
{
    postgres.WithDataVolume();
    rabbitmq.WithDataVolume();
    keycloak.WithDataVolume();
}

// projects
var catalog = builder.AddProject<Projects.CatalogApi>("catalogapi")
       .WithReference(catalogDb)
       .WithReference(rabbitmq)
       .WaitFor(catalogDb)
       .WaitFor(rabbitmq)
       .WithSwaggerUi();
       //.AsContainerApp();

var basket = builder.AddProject<Projects.BasketApi>("basketapi")
    .WithReference(cache)
    .WithReference(catalog) // For Service Discovery. Helps to access GetProductById and get the price of the product
    .WithReference(rabbitmq)
    .WithReference(keycloak)
    .WaitFor(cache)
    .WaitFor(rabbitmq)
    .WaitFor(keycloak)
    .WithSwaggerUi();


var webapp = builder.AddProject<Projects.WebApp>("webapp")
                    .WithExternalHttpEndpoints() 
                    .WithReference(cache)
                    .WithReference(catalog)
                    .WithReference(basket)
                    .WaitFor(catalog)
                    .WaitFor(basket);

builder.Build().Run();
