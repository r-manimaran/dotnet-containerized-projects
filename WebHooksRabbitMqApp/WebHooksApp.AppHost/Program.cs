var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
                       .WithDataVolume()
                       .WithPgAdmin()
                       .AddDatabase("webhooks");

var queue = builder.AddRabbitMQ("rabbitmq")
                      .WithDataVolume()
                      .WithManagementPlugin();

builder.AddProject<Projects.Orders_Api>("orders-api")
        .WithReference(postgres)
        .WaitFor(postgres)
        .WithReference(queue)
        .WaitFor(queue);

builder.Build().Run();
