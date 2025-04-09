using BusinesscardsApi.Data;
using BusinesscardsApi.Extensions;
using BusinesscardsApi.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader()
                   .WithExposedHeaders("X-Pagination");
        });
});

builder.Services.AddControllers();

builder.Services.AddScoped<IBusinessCardService, BusinessCardService>();

builder.Services.AddOpenApi();

var connectionString = builder.Configuration.GetConnectionString("default"); ;
builder.Services.AddDbContext<AppDbContext>((_,options) =>
{
    options.UseNpgsql(connectionString, npgsqlOptions =>
    {
        //npgsqlOptions.MigrationsHistoryTable(Data)
    });
    options.UseSnakeCaseNamingConvention();
});

builder.Services.AddScoped<IQRCodeService, QRCodeService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
app.UseSwaggerUI(opt =>
    opt.SwaggerEndpoint("/openapi/v1.json", "OpenAPI V1"));

app.UseCors("AllowAll");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

await app.ApplyMigrations();

await app.RunAsync();
