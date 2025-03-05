using CommunityToolkit.Aspire.Hosting.Dapr;

var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.EShop_OrderServiceApi>("dapr-eshop-orders-api")
        .WithDaprSidecar(new DaprSidecarOptions
        {
            AppId="orders-api"
        });

builder.AddProject<Projects.Eshop_CheckoutServiceApi>("dapr-eshop-checkout-api")
    .WithDaprSidecar("checkout-api");

builder.Build().Run();
