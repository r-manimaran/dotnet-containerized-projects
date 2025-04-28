using MoviesApi.Models;

namespace MoviesApi.Services;

public interface IMovieService 
{
    Task<List<Movie>> GetMoviesAsync(string? term=null, int limit=10);

    Task UpdateMoviesAsync();
}
