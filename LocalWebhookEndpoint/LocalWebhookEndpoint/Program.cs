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

app.MapPost("/webhooks", (HttpContext context) =>
{
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine("--> We have a webhook hit");

    var headers = context.Request.Headers;
    foreach(var header in headers)
    {
        Console.WriteLine($"{header.Key} / {header.Value}");       
    }
    Console.ResetColor();

    return Results.Ok();
});

app.Run();

