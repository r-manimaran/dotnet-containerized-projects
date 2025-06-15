var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
                       .WithPgAdmin()
                       .WithDataVolume()
                       .AddDatabase("Appreciation");

var username = builder.AddParameter("username", "admin");

var password = builder.AddParameter("password","admin", secret: true);

var blobs = builder.AddAzureStorage("storage")
                   .RunAsEmulator()
                   .AddBlobs("blobs");


var keycloak = builder.AddKeycloak("keycloak", 8082)
    .WithDataVolume()
    .WithExternalHttpEndpoints()
    .WithRealmImport("./KeycloakConfiguration");

    
var api = builder.AddProject<Projects.AppreciateAppApi>("appreciateappapi")
                .WithExternalHttpEndpoints()
                 .WithReference(postgres)
                 .WaitFor(postgres)
                 .WithReference(keycloak)
                 .WithEnvironment("Keycloak__ClientId", "confidential-client")
                 .WithEnvironment("keycloak__ClientSecret", "ze4SQDpbyBlB72kdTCTv8ecSWsJHf2Js")
                 .WithReference(blobs);

builder
    .AddNpmApp("AngularFrontEnd", "../Appreciation.Web")
    .WithReference(api)
    .WithEndpoint(4200,scheme:"http",env:"PORT")
    .PublishAsDockerFile();
    

builder.Build().Run();
