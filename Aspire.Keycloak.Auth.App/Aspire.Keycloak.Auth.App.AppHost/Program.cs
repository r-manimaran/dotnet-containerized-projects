var builder = DistributedApplication.CreateBuilder(args);
var username = builder.AddParameter("username", "admin");
var password = builder.AddParameter("password","admin", secret: true);
var keycloak = builder.AddKeycloak("keycloak", 8080, username, password)
               .WithDataVolume()
               .WithRealmImport("./KeycloakConfiguration");
                //.WithRealImport("./KeycloakConfiguration/Test-users.json");

//var keycloak = builder.AddKeycloakContainer("keycloak", "keycloak", username, password, 8080)
//                      .WithDataVolume()
//                      .WithImport("./KeycloakConfiguration/Test-realm.json")
//                      .WithImport("./KeycloakConfiguration/Test-users.json");

//var realm = keycloak.AddRealm("Test");


builder.AddProject<Projects.Aspire_Keycloak_Auth_Api>("aspire-keycloak-auth-api")
            .WithReference(keycloak)
            .WithEnvironment("Keycloak__ClientId", "confidential-client")
            .WithEnvironment("keycloak__ClientSecret", "ze4SQDpbyBlB72kdTCTv8ecSWsJHf2Js");
            

builder.Build().Run();
