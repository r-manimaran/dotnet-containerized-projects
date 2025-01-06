var builder = DistributedApplication.CreateBuilder(args);

var password = builder.AddParameter("password", secret:true);

var sql = builder.AddSqlServer("sqldb", password, 40796)
                .WithBindMount("./sqlserverconfig", "/usr/config")
                .WithBindMount("./DBContainer", "/docker-entrypoint-initdb.d")
                .WithEntrypoint("/usr/config/entrypoint.sh")
                .WithLifetime(ContainerLifetime.Session);

// sql.AddDatabase("TestDB");
builder.AddProject<Projects.SQLApi>("sqlapi")
        .WithReference(sql)
        .WaitFor(sql);

builder.Build().Run();
