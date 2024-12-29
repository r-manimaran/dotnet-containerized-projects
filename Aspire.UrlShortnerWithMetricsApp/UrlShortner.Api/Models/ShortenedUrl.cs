namespace UrlShortner.Api.Models;

public record ShortenedUrl(string ShortCode, string OriginalUrl, DateTime createdOn);
