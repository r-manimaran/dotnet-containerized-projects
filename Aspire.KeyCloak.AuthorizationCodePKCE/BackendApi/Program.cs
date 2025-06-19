var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var authorityUrl = builder.Configuration["oidc:Authority"] ?? "https://localhost:5001";
var audience = builder.Configuration["oidc:Audience"] ?? "api1";

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.Authority = authorityUrl;
        options.Audience = audience;
        options.RequireHttpsMetadata = false; // Set to true in production
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = authorityUrl,
            ValidateAudience = true,
            ValidAudience = audience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero // Reduce the clock skew to zero for testing purposes
        };
    });
builder.Services.AddCors();

builder.Services.AddAuthorization();
var app = builder.Build();

// Configure the HTTP request pipeline.

//app.UseHttpsRedirection();
app.UseAuthentication();

app.UseAuthorization();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("api/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
}).RequireAuthorization();

app.MapGet("/api/time", () =>
{
    return Results.Ok(DateTime.Now);

}).RequireAuthorization();

app.UseCors(policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
