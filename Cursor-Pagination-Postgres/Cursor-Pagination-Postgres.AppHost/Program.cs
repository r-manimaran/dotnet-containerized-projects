var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
                      .WithPgAdmin()
                      .WithDataVolume();

var productsdb = postgres.AddDatabase("productsdb");

builder.AddProject<Projects.ProductsApi>("productsapi")
       .WithReference(productsdb)
       .WaitFor(productsdb);

builder.Build().Run();
