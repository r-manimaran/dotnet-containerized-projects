var builder = DistributedApplication.CreateBuilder(args);

//var password = builder.AddParameter("password", secret: true);
var rabbit = builder.AddRabbitMQ("messaging").WithManagementPlugin();

builder.AddProject<Projects.AspireDemo_Api>("aspiredemo-api")
    .WithReference(rabbit);

builder.AddProject<Projects.AspireDemo_RabbitMQConsumer>("aspiredemo-consumer")
    .WithReference(rabbit);

builder.Build().Run();
