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

builder.AddProject<Projects.Webhooks_Processing>("webhooks-processing")
        .WithReplicas(3)
        .WithReference(postgres)
        .WaitFor(postgres)
        .WithReference(queue)
        .WaitFor(queue);

builder.Build().Run();
