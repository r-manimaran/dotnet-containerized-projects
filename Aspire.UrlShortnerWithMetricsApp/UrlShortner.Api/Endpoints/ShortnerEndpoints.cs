using Carter;
using UrlShortner.Api.Services;

namespace UrlShortner.Api.Endpoints;

public class ShortnerEndpoints : CarterModule
{
    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("shorten", CreateShortenUrl);

        app.MapGet("{shortcode}", Redirect);

        app.MapGet("urls", GetAllUrls);
    }

    private async Task<IResult> CreateShortenUrl(string url, IUrlShortnerService urlShortnerService)
    {
        if (!Uri.TryCreate(url, UriKind.Absolute, out _))
        {
            return Results.BadRequest("Invalid URL format.");
        }
        var shortCode = urlShortnerService.ShortenedUrl(url);

        return Results.Ok(new { shortCode });
    }

    private async Task<IResult> Redirect(string shortcode, IUrlShortnerService urlShortnerService)
    {
        var originalUrl = await urlShortnerService.GetOriginalUrl(shortcode);
        
        return originalUrl is null ? Results.NotFound() : Results.Redirect(originalUrl);

    }

    private async Task<IResult> GetAllUrls(IUrlShortnerService urlShortnerService)
    {
        var response = await urlShortnerService.GetAllUrls();
        return Results.Ok(response);
    }
}
