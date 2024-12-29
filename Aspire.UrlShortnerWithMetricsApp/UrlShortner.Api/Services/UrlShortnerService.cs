using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Hybrid;
using Npgsql;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using UrlShortner.Api.Models;

namespace UrlShortner.Api.Services;

internal sealed class UrlShortnerService : IUrlShortnerService
{
    private readonly NpgsqlDataSource _dataSource;
    private readonly ILogger<UrlShortnerService> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly HybridCache _hybridCache;
    private const int MaxRetries = 3;

    // For Metrics
    private static readonly Meter Meter = new Meter("UrlShortening.Api");
    private static readonly Counter<int> RedirectsCounter = Meter.CreateCounter<int>(
        "url_shortener.redirects",
        "The no. of successful redirects");
    private static readonly Counter<int> FailedRedirectsCounter = Meter.CreateCounter<int>(
        "url_shortener.failed_redirects",
        "The no. of failed redirects");
    public UrlShortnerService(NpgsqlDataSource dataSource, 
                            ILogger<UrlShortnerService> logger,
                            IHttpContextAccessor httpContextAccessor,
                            HybridCache hybridCache)
    {
        _dataSource = dataSource;
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
        _hybridCache = hybridCache;
    }
    public async Task<string> ShortenedUrl(string originalUrl)
    {
        for (int attempt = 0; attempt < MaxRetries; attempt++)
        {
            try
            {
                var shortCode = GenerateShortCode();
                const string sql =
                    """
            INSERT INTO shortened_urls (short_code, original_url)
            VALUES (@ShortCode, @OriginalUrl)
            RETURNING short_code;
            """;
                await using var connection = await _dataSource.OpenConnectionAsync();

                var result = await connection.QuerySingleAsync<string>(
                            sql,
                            new { ShortCode = shortCode, OriginalUrl = originalUrl });

                // Add the Shortcode and Original Url to cache
                await _hybridCache.SetAsync(shortCode, originalUrl);
                _logger.LogInformation("Created the ShortenedUrl and updated in Cache {result}",result);
                return result;
            }
            catch (PostgresException ex) when (ex.SqlState == PostgresErrorCodes.UniqueViolation)
            {
                if (attempt == MaxRetries)
                {
                    _logger.LogError(ex,
                        "Failed to generate unique short code after {MaxRetries} attempts",
                        MaxRetries);
                    throw new InvalidOperationException("Failed to generate unique short code", ex);
                }

                _logger.LogWarning(
                    "Short code collision occured. Retrying .. {Attempt} of {MaxAttempt}",
                    attempt + 1,
                    MaxRetries);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,"Failed.");
            }
        }
        throw new InvalidOperationException("Failed to generate shortned Url.");
    }

    private static string GenerateShortCode()
    {
        const int length = 7;
        const string alphaNumerics = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";

        // Get Random char from the above alphaNumerics
        var chars =Enumerable.Range(0, length)
                        .Select(_=> alphaNumerics[Random.Shared.Next(alphaNumerics.Length)])
                        .ToArray();

        return new string(chars);
    }
    private async Task RecordVisit(string shortCode)
    {
        var context = _httpContextAccessor.HttpContext;
        var userAgent = context?.Request.Headers.UserAgent.ToString();
        var referer = context?.Request.Headers.Referer.ToString();

        const string sql =
            """
            INSERT INTO url_visits (short_code,user_agent,referer)
            VALUES (@ShortCode, @UserAgent,@Referer)            
            """;
        await using var connection = await  _dataSource.OpenConnectionAsync();
        await connection.ExecuteAsync(
            sql,
            new
            {
                ShortCode = shortCode,
                UserAgent = userAgent,
                Referer = referer
            });
    }
    public async Task<string?> GetOriginalUrl(string shortCode)
    {
      var originalUrl =  await _hybridCache.GetOrCreateAsync(shortCode,async token=>
        {
            // Cache Miss- Get from Database
            _logger.LogTrace("Cache miss for shortCode: {shortCode}",shortCode);
            const string sql =
                      """
                        SELECT original_url
                        FROM shortened_urls
                        WHERE short_code=@ShortCode;
                        """;
            await using var connection = await _dataSource.OpenConnectionAsync(token);

            var originalUrl = await connection.QueryFirstOrDefaultAsync<string>(
                                sql,
                                new { ShortCode = shortCode });
            return originalUrl;
        });

        // For Metrics
        if (originalUrl is null)
        {
            FailedRedirectsCounter.Add(1, new TagList { { "short_code", shortCode } });
        }
        else
        {
            await RecordVisit(shortCode);
            RedirectsCounter.Add(1, new TagList { { "short_code", shortCode } });
        }
        

        _logger.LogTrace("Found in cache for shortCode:{shortCode}", shortCode);

        return originalUrl;
      
    }

    public async Task<IEnumerable<ShortenedUrl>> GetAllUrls()
    {
        const string sql =
            """
            SELECT short_code as ShortCode, original_url as OriginalUrl, created_on as CreatedOn
            FROM shortened_urls
            ORDER BY created_on DESC;
            """;
        await using var connection = await _dataSource.OpenConnectionAsync();
        return await connection.QueryAsync<ShortenedUrl>(sql);
    }

}
