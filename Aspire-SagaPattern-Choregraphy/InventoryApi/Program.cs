using InventoryApi.Data;
using InventoryApi.Endpoints;
using InventoryApi.Models;
using Microsoft.EntityFrameworkCore;
using SharedLib.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddDbContext<InventoryDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("inventorydb"));
});

builder.Services.AddOpenApi();

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(opt =>
    {
        opt.SwaggerEndpoint("/openapi/v1.json", "OpenApi V1");
    });

    // Apply the Migration
    await app.ApplyMigrationAsync<InventoryDbContext>(seedData: true,
        seedDataFunc: async context =>
        {
            if (!await context.Products.AnyAsync())
            {
                await context.Products.AddRangeAsync(
                    new Product { Name = "Product 1", Price = 10.99m, Quantity = 100, CreatedBy = "system", ModifiedBy = "system", CreatedOn = DateTime.UtcNow, ModifiedOn = DateTime.UtcNow },
                            new Product { Name = "Product 2", Price = 20.99m, Quantity = 50, CreatedBy = "system", ModifiedBy = "system", CreatedOn = DateTime.UtcNow, ModifiedOn = DateTime.UtcNow }
                    );
                await context.SaveChangesAsync();
            }
        });
}
app.UseHttpsRedirection();

app.MapProductsEndpoints();

app.Run();
