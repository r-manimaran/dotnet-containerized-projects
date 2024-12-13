using Aspire.Keycloak.Auth.Api;
using Dapper;
using Npgsql;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.AddNpgsqlDataSource("stocks");

builder.Services.AddControllers();

builder.Services.AddOpenApi();

builder.Services.AddProblemDetails();

builder.Services.AddHostedService<DatabaseInitializer>();

// Add KeyCloak Authentication
builder.Services.AddAuthentication()
        .AddKeycloakJwtBearer("keycloak", "maransys", options =>
        {
            options.RequireHttpsMetadata = false;
            options.Audience = "account";
        });

var app = builder.Build();

app.UseExceptionHandler();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.MapGet("/stock-prices", async (NpgsqlDataSource dataSource) =>
{
    using var connection = await dataSource.OpenConnectionAsync();
    var stockPrices = await connection.QueryAsync("SELECT * FROM public.stocks");
    return Results.Ok(stockPrices);
})
.RequireAuthorization(); // this will apply [Authorize] attribute to the API endpoint

app.Run();
