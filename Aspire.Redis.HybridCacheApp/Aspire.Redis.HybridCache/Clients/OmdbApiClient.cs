using Aspire.Redis.HybridCache.Configuration;
using Aspire.Redis.HybridCache.Models;

namespace Aspire.Redis.HybridCache.Clients;

public class OmdbApiClient
{
    private readonly HttpClient _httpClient;
    private readonly OmdbApiSettings _settings;
    private readonly ILogger<OmdbApiClient> _logger;

    public OmdbApiClient(HttpClient httpClient, IConfiguration configuration, 
                         ILogger<OmdbApiClient> logger)
    {
      _httpClient = httpClient;
      _logger = logger;
      _settings = configuration.GetSection("OmdbApi").Get<OmdbApiSettings>() ??
                throw new InvalidOperationException("OmdbApi settings not found");
    }
    
   
    public async Task<Movie?> GetMovieByImdbIdAsync(string imdbId, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Fetching movie {imdbId} from OMDB API", imdbId);
            var response = await _httpClient.GetAsync(
                $"?apikey={_settings.OmdbApiKey}&i={imdbId}",
                cancellationToken
            );
            if(!response.IsSuccessStatusCode)
            {
                return null;
            }
            var movie = await response.Content.ReadFromJsonAsync<Movie>(cancellationToken: cancellationToken);
            return movie;
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Error fetching movie {imdbId} from OMDB API", imdbId);
            Console.WriteLine(ex.Message);
            return null;
        }
    }
}