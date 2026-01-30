using Bogus;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwaggerUI(options => {
        options.SwaggerEndpoint(
        "/openapi/v1.json", "OpenAPI v1");
    });

}

app.UseHttpsRedirection();

app.MapGet("/products", () =>
{
    var records = GenerateRecords(10).ToList();
    return records;
})
.WithName("GetProducts");

app.Run();
static IEnumerable<Product> GenerateRecords(int count)
{
    var faker = new Faker<Product>()
    .RuleFor(p => p.Id, f => Guid.NewGuid())
    .RuleFor(p => p.Name, f => f.Commerce.ProductName())
    .RuleFor(p => p.Price, Random.Shared.Next(10,99));

    return faker.Generate(count);
}

public class Product
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public float Price { get; set; }
}
