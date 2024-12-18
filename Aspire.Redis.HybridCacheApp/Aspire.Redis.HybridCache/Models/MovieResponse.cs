namespace Aspire.Redis.HybridCache.Models
{
    public class MovieResponse
    {
        public Movie Movie { get; set; }
        public int ApiRequestCount { get; set; }
    }
}
