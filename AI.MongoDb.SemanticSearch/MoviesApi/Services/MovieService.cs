using MongoDB.Driver;
using MoviesApi.Models;

namespace MoviesApi.Services;

public class MovieService : IMovieService
{
    private readonly IMongoCollection<Movie> _moviesCollection;
    public MovieService(IMongoClient mongoClient)
    {
        var database = mongoClient.GetDatabase("sample_mflix");

        _moviesCollection = database.GetCollection<Movie>("movies");
    }
    public async Task<List<Movie>> GetMoviesAsync(string? term, int limit=10)
    {
        var filter = Builders<Movie>.Filter.Empty;

        if(!string.IsNullOrEmpty(term))
        {
            filter = Builders<Movie>.Filter
                        .Where(m => m.Title.ToLower().Contains(term.ToLower()));
        }

        return await _moviesCollection
                .Find(filter)
                .Limit(limit)
                .ToListAsync();
    }
}
