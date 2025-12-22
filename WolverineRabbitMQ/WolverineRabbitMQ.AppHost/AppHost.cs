using Aspire.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
                      .WithDataVolume()
                      .AddDatabase("user-mgmt");
                     

var rmq = builder.AddRabbitMQ("rmq")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithManagementPlugin();

builder.AddProject<Projects.UserManagementApi>("usermanagementapi")
    .WithReference(postgres)
    .WaitFor(postgres)
    .WithReference(rmq)
    .WaitFor(rmq);

builder.Build().Run();
