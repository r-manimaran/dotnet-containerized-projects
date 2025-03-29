var builder = DistributedApplication.CreateBuilder(args);

var password = builder.AddParameter("RabbitMQPassword", true);

var rabbit = builder.AddRabbitMQ("messaging", password:password)
                    .WithManagementPlugin();

var apiService = builder.AddProject<Projects.Aspire_BackendApi>("aspire-backendapi")
                        .WithReference(rabbit)
                        .WithReference("external-api",new Uri("https://webhook.site"));

builder.Build().Run();
