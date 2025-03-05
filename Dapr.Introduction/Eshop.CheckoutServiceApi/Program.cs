using Dapr.Client;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddOpenApi();

builder.Services.AddDaprClient();

var app = builder.Build();

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapPost("/create-order", async (OrderRequest request, DaprClient client) =>
{
    var result = await client.InvokeMethodAsync<OrderRequest, string>(
                "orders-api",
                "process-order",
                request);
    return Results.Ok(result);
});

app.UseHttpsRedirection();

app.Run();
public record OrderRequest(string OrderId, string CustomerId, List<string> Items);