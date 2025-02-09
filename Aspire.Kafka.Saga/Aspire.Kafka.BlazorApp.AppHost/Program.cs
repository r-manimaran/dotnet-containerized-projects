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


var productApi = builder.AddProject<Projects.ProductsApi>("productsapi")
                       .WithReference(messaging)
                       .WaitFor(messaging)
                       .WithReference(postgres)
                       .WaitFor(postgres);
                    

var orderApi = builder.AddProject<Projects.OrdersApi>("ordersapi")
                     .WithReference(messaging)
                     .WaitFor(messaging)
                     .WithReference(sqlserver)
                     .WaitFor(sqlserver);

var paymentApi = builder.AddProject<Projects.PaymentApi>("paymentapi")
                        .WithReference(messaging)
                        .WaitFor(messaging);

builder.AddProject<Projects.WebApp>("webapp")
       .WithReference(messaging)
       .WaitFor(messaging)
       .WithReference(productApi)
       .WaitFor(productApi)
       .WithReference(orderApi)
       .WaitFor(orderApi)
       .WithReference(paymentApi)
       .WaitFor(paymentApi);
   

builder.Build().Run();
