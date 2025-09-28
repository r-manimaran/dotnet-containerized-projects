using Aspire.AI.SQLServer.Data;
using Aspire.AI.SQLServer.Endpoints;
using Aspire.AI.SQLServer.Extensions;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddOpenApi();


builder.Services.AddDbContext<AppDbContext>(opt=> {

   opt.UseSqlServer(builder.Configuration.GetConnectionString("TodoDb"),o=>o.UseCompatibilityLevel(170));
    //opt.UseAzureSql(builder.Configuration.GetConnectionString("TodoDb"));
});

var app = builder.Build();

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwaggerUI(options => {
        options.SwaggerEndpoint(
        "/openapi/v1.json", "OpenAPI v1");
    });

}
app.ApplyMigrations();

app.UseHttpsRedirection();

app.MapUserEndpoints();

app.Run();

