using UrlShortner.Api.Models;

namespace UrlShortner.Api.Services;

internal interface IUrlShortnerService
{
    Task<IEnumerable<ShortenedUrl>> GetAllUrls();
    Task<string?> GetOriginalUrl(string shortCode);
    Task<string> ShortenedUrl(string originalUrl);
}