using CatalogApi.Data;


var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddOpenApi();

builder.AddNpgsqlDbContext<ProductDbContext>("catalogdb");

builder.Services.AddScoped<IProductService, ProductService>();

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseSwaggerUI(options => {
    options.SwaggerEndpoint(
    "/openapi/v1.json", "OpenAPI v1");
});

app.UseMigration();

app.UseHttpsRedirection();

app.MapProductsEndpoints();

app.Run();


