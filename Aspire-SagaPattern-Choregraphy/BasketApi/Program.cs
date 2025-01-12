using BasketApi.Data;
using BasketApi.Extensions;
using Carter;
using Microsoft.EntityFrameworkCore;
using SharedLib.Extensions;
using SharedLib.Repository;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.MapMessaging();

builder.Services.AddDbContext<BasketDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("orders"));
});

builder.Services.AddScoped<DbContext, BasketDbContext>();
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

builder.Services.AddCarter();

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

    // Apply Migration
    await app.ApplyMigrationAsync<BasketDbContext>();
}

app.UseHttpsRedirection();

app.MapCarter();

app.Run();

