var builder = DistributedApplication.CreateBuilder(args);

// Resources
var postgres = builder.AddPostgres("postgres")
                      .WithDataVolume()
                      .WithPgAdmin()
                      .AddDatabase("inventorydb");

var sqlserverPassword = builder.AddParameter("sqlserverPassword", secret: true);
var sqlserver = builder.AddSqlServer("sqldb",sqlserverPassword,40796)
                       .WithDataVolume()
                       .AddDatabase("orders");

var messageQueuePassword = builder.AddParameter("messageQueuePassword", secret:true);

var rabbitmq = builder.AddRabbitMQ("messaging", password: messageQueuePassword)
                      .WithManagementPlugin();


builder.AddProject<Projects.InventoryApi>("inventoryapi")
       .WithReference(postgres)
       .WaitFor(postgres)
       .WithReference(rabbitmq);

builder.AddProject<Projects.BasketApi>("basketapi")
        .WithReference(sqlserver)
        .WaitFor(sqlserver)
        .WithReference(rabbitmq);

builder.AddProject<Projects.OrderApi>("orderapi")
       .WithReference(sqlserver)
       .WaitFor(sqlserver)
       .WithReference(rabbitmq); ;

builder.AddProject<Projects.PaymentApi>("paymentapi")
       .WithReference(rabbitmq); 


builder.Build().Run();
