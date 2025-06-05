
var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.MapGet("/hello", () => 
{
    return("Hello, World!");
});

app.Run();

