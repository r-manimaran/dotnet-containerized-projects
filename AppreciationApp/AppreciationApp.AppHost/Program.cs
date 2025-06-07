var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
                       .WithPgAdmin()
                       .WithDataVolume()
                       .AddDatabase("Appreciation");

var api = builder.AddProject<Projects.AppreciateAppApi>("appreciateappapi")
                 .WithReference(postgres)
                 .WaitFor(postgres);

builder
    .AddNpmApp("AngularFrontEnd", "../Appreciation.Web")
    .WithReference(api)
    .WithEndpoint(4200,scheme:"http",env:"PORT")
    .PublishAsDockerFile();

    

builder.Build().Run();
