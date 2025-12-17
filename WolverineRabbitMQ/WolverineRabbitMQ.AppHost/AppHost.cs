var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
                      .WithDataVolume()
                      .AddDatabase("user-mgmt");

builder.AddProject<Projects.UserManagementApi>("usermanagementapi")
    .WithReference(postgres)
    .WaitFor(postgres);

builder.Build().Run();
