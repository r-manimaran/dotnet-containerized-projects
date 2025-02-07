var builder = DistributedApplication.CreateBuilder(args);

// Kafka messaging
var messaging = builder.AddKafka("messaging")
                      .WithKafkaUI();

// Postgres for products
var postgres = builder.AddPostgres("postgres")
                      .WithDataVolume()
                      .WithPgAdmin();
postgres.AddDatabase("productsdb");

// sql server for orders

var sqlserver = builder.AddSqlServer("sqlserver")
                       .WithDataVolume();
sqlserver.AddDatabase("ordersdb");





var productapi = builder.AddProject<Projects.ProductsApi>("productsapi")
                       .WithReference(messaging)
                       .WaitFor(messaging)
                       .WithReference(postgres)
                       .WaitFor(postgres);
                    

var orderapi = builder.AddProject<Projects.OrdersApi>("ordersapi")
                     .WithReference(messaging)
                     .WaitFor(messaging)
                     .WithReference(sqlserver)
                     .WaitFor(sqlserver);

builder.AddProject<Projects.WebApp>("webapp")
       .WithReference(productapi)
       .WaitFor(productapi)
       .WithReference(orderapi)
       .WaitFor(orderapi)
       .WithReference(messaging)
       .WaitFor(messaging);

builder.Build().Run();
