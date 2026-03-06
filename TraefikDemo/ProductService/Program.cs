var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// app.UseHttpsRedirection(); // Disabled for Traefik

app.MapGet("/api/products", () =>
{
    var products = new[]
     {
        new { Id = 1, Name = "Laptop",  Price = 999.99 },
        new { Id = 2, Name = "Phone",   Price = 499.99 },
        new { Id = 3, Name = "Headset", Price = 79.99  }
    };
    return Results.Ok(products);
});

app.MapGet("/api/products/{id:int}", (int id) =>
    Results.Ok(new { Id = id, Name = "Laptop", Price = 999.99 }));

app.Run();

