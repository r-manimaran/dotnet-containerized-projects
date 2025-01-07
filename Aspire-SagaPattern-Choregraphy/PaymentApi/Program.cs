using Microsoft.EntityFrameworkCore;
using PaymentApi.Data;
using PaymentApi.Endpoints;
using SharedLib.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddDbContext<PaymentDbContext>(options =>
{
    options.UseSqlite(builder.Configuration.GetConnectionString("Default"));
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

    await app.ApplyMigrationAsync<PaymentDbContext>();
}

app.UseHttpsRedirection();

app.MapPaymentEndpoints();

app.Run();

