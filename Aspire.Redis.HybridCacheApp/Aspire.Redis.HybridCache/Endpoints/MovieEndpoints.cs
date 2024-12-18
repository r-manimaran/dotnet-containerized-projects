using Aspire.Redis.HybridCache.Clients;
using caching = Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Http.HttpResults;
using Aspire.Redis.HybridCache.Models;

namespace Aspire.Redis.HybridCache.Endpoints
{
    public static class MovieEndpoints
    {
        [EndpointDescription("This is the description for API endpoint")]
        [EndpointSummary("Summary of the endpoint")]
        public static void MapMovieEndpoints(this IEndpointRouteBuilder endpointRoute)
        {
            endpointRoute.MapGet("/movies/{imdbId}", async (string imdbId, 
                                                                   OmdbApiClient client,
                                                                   IMemoryCache cache,
                                                                   CancellationToken cancellationToken) =>
            {

                var cachedMovie =await cache.GetOrCreateAsync($"movies-{imdbId}", async entry =>
                {
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
                    var movie = await client.GetMovieByImdbIdAsync(imdbId, cancellationToken);
                    return movie;
                });                
                return cachedMovie is null ? Results.NotFound() : Results.Ok(cachedMovie);
            })
                .WithName("GetMovie")
                .WithTags("Movies")
                .WithOpenApi();




            endpointRoute.MapGet("/movies/hybrid/{imdbId}", async (string imdbId,
                                                                   OmdbApiClient client,
                                                                   caching.HybridCache cache,
                                                                   CancellationToken cancellationToken) =>
            {
                var cachedMovie = await cache.GetOrCreateAsync($"movies-{imdbId}", async token =>
                {
                    var movie = await client.GetMovieByImdbIdAsync(imdbId, token);
                    return movie;
                },
                tags:["movies"],
                cancellationToken: cancellationToken);

                return cachedMovie is null ? Results.NotFound() : Results.Ok(cachedMovie);

            })
                .WithName("GetMovieHybrid")
                .WithTags("Movies")
                .WithOpenApi();

            endpointRoute.MapDelete("/movies/hybrid/{imdbId}/invalidate-cache", async (string imdbId,
                                                                   caching.HybridCache cache,
                                                                   CancellationToken cancellationToken) =>
            {
                await cache.RemoveAsync($"movies-{imdbId}", cancellationToken);

                //Tag based evaction, which removes both Local cache and distrributed cache based on tags
                // await cache.RemoveByTagAsync(["movies"]);

                return Results.NoContent();

            })
                .WithName("InvalidateCacheHybrid")
                .WithTags("Movies")
                .WithOpenApi();

            //Two endpoints to explore Stempade 
            endpointRoute.MapGet("/movies/{imdbId}/unsafe", async (string imdbId,
                                                                   OmdbApiClient client,
                                                                   IMemoryCache cache,
                                                                   CancellationToken cancellationToken) =>
            {
                //RequestCounter.Reset(imdbId);

                var cachedMovie = await cache.GetOrCreateAsync($"movies-{imdbId}", async entry =>
                {
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);

                    //simulate some delay
                    await Task.Delay(2000,cancellationToken);

                    var movie = await client.GetMovieByImdbIdAsync(imdbId, cancellationToken);
                    if (movie == null) return null;
                    //RequestCounter.IncrementAndGet(imdbId);
                    return movie;
                });

                var response = new MovieResponse
                {
                    Movie = cachedMovie,
                    //ApiRequestCount = RequestCounter.Get(imdbId)
                };
                return cachedMovie is null ? Results.NotFound() : Results.Ok(response);
            })
                .WithName("GetMovieUnsafe")
                .WithTags("Movies")
                .WithOpenApi();

            endpointRoute.MapGet("/movies/hybrid/{imdbId}/safe", async (string imdbId,
                                                                     OmdbApiClient client,
                                                                     caching.HybridCache cache,
                                                                     CancellationToken cancellationToken) =>
            {
                var cachedMovie = await cache.GetOrCreateAsync($"movies-{imdbId}", async token =>
                {
                    var movie = await client.GetMovieByImdbIdAsync(imdbId, token);
                    return movie;
                },
                tags: ["movies"],
                cancellationToken: cancellationToken);

                return cachedMovie is null ? Results.NotFound() : Results.Ok(cachedMovie);

            })
                  .WithName("GetMovieHybridSafe")
                  .WithTags("Movies")
                  .WithOpenApi();
        }
    }
}
