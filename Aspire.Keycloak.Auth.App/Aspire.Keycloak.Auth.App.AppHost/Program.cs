var builder = DistributedApplication.CreateBuilder(args);


// Configure Postgres Database
var postgres = builder.AddPostgres("postgres").WithDataVolume();
//var postgres = builder.AddPostgres("postgres").WithDataBindMount(
//                source:@"C:\PostgreSQL\Data",isReadOnly:false);

var stocksdb = postgres.AddDatabase("stocks");

// configure Keycloak
var username = builder.AddParameter("username", "admin");
var password = builder.AddParameter("password","admin", secret: true);
var keycloak = builder.AddKeycloak("keycloak", 8080, username, password)
               .WithDataVolume()
               .WithExternalHttpEndpoints()
               .WithRealmImport("./KeycloakConfiguration");
                //.WithRealImport("./KeycloakConfiguration/Test-users.json");

//var keycloak = builder.AddKeycloakContainer("keycloak", "keycloak", username, password, 8080)
//                      .WithDataVolume()
//                      .WithImport("./KeycloakConfiguration/Test-realm.json")
//                      .WithImport("./KeycloakConfiguration/Test-users.json");

//var realm = keycloak.AddRealm("Test");


var apiService= builder.AddProject<Projects.Aspire_Keycloak_Auth_Api>("aspire-keycloak-auth-api")
            .WithExternalHttpEndpoints()
            .WithReference(stocksdb)
            .WithReference(keycloak)
            .WithEnvironment("Keycloak__ClientId", "confidential-client")
            .WithEnvironment("keycloak__ClientSecret", "ze4SQDpbyBlB72kdTCTv8ecSWsJHf2Js");


builder.AddProject<Projects.Aspire_FrontEnd>("aspire-frontend")
    .WithReference(keycloak)
    .WithReference(apiService)
    .WaitFor(keycloak)
    .WaitFor(apiService);
            

builder.Build().Run();
