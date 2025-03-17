using Aspire.eshop.CatalogDb.Data;
using Aspire.eshop.CatalogService.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.AddNpgsqlDbContext<CatalogDbContext>("catalogdb");

builder.Services.AddControllers();

builder.Services.AddOpenApi();

builder.Services.AddProblemDetails();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/openapi/v1.json", "Openapi v1");
});
app.UseHttpsRedirection();

app.MapCatalogApiEndpoints();

app.UseAuthorization();

app.MapControllers();

app.Run();
