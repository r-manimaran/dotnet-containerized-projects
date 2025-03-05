var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddOpenApi();

var app = builder.Build();

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapPost("/process-order", async (OrderRequest request) =>
{
    await ProcessOrderAsync(request);

    return Results.Ok($"Order {request.OrderId} confirmation :# {Guid.NewGuid().ToString()[..8]}");
});

app.Run();

async Task ProcessOrderAsync(OrderRequest request)
{
    // Process the Order
    await Task.Delay(100);
}

public record OrderRequest(string OrderId, string CustomerId, List<string> Items);