using MongoDB.Driver;
using MoviesApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors();

builder.Services.AddSingleton<IMongoClient>(_ =>
    new MongoClient(builder.Configuration.GetConnectionString("mongodb")));

builder.Services.AddScoped<IMovieService, MovieService>();

builder.Services.AddControllers();

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseCors(policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

app.MapGet("/api/movies", async (IMovieService movieService, string? term = null, int limit = 10) =>
{
    var movies = await movieService.GetMoviesAsync(term, limit);
    return Results.Ok(movies);
});

app.UseAuthorization();

app.MapControllers();

app.Run();
