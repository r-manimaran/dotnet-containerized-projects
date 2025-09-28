var builder = DistributedApplication.CreateBuilder(args);

var sql = builder.AddSqlServer("sql")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithImageTag("2025-latest")
    .WithEnvironment("ACCEPT_EULA", "Y")
    .WithHostPort(5678);

var TodoDb = sql.WithDataVolume().AddDatabase("TodoDb");

builder.AddProject<Projects.Aspire_AI_SQLServer>("aspire-ai-sqlserver")
     .WithReference(TodoDb);

builder.Build().Run();
