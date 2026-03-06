var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// app.UseHttpsRedirection(); // Disabled for Traefik

app.MapGet("/api/orders", () =>
{
    var orders = new[]
    {
        new { Id = 101, ProductId = 1, Quantity = 2, Status = "Shipped"  },
        new { Id = 102, ProductId = 3, Quantity = 1, Status = "Pending"  }
    };
    return Results.Ok(orders);
});

app.MapPost("/api/orders", (dynamic order) =>
    Results.Created("/api/orders/103", new { Id = 103, Status = "Created" }));

app.Run();

