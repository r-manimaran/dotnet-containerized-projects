using Microsoft.EntityFrameworkCore;
using UserManagementApi.Data;
using UserManagementApi.Endpoints;
using Wolverine;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Host.UseWolverine();

// Add DbContext
var connString = builder.Configuration.GetConnectionString("user-mgmt");

builder.Services.AddDbContext<AppDbContext>(opts=> opts.UseNpgsql(connString));

builder.Services.AddOpenApi();

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwaggerUI(options => {
        options.SwaggerEndpoint("/openapi/v1.json", "OpenAPI v1");
    });

    // Migration
    var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();

}
app.MapEndpoints();

app.UseHttpsRedirection();

app.Run();

