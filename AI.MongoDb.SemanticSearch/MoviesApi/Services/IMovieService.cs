using MoviesApi.Models;

namespace MoviesApi.Services;

public interface IMovieService 
{
    Task<List<Movie>> GetMoviesAsync(string? term, int limit=10);
}
