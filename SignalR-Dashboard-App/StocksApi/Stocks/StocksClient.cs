using Microsoft.Extensions.Caching.Memory;
using System.Text.Json;

namespace StocksApi.Stocks;

public sealed class StocksClient(HttpClient httpClient,
    IConfiguration configuration,
    IMemoryCache cache,
    ILogger<StocksClient> logger)
{
    public async Task<StockPriceResponse?> GetDataForTicker(string ticker)
    {
        StockPriceResponse? stockPriceResponse = await cache.GetOrCreateAsync($"stocks-{ticker}", async entry =>
        {
            entry.SetAbsoluteExpiration(TimeSpan.FromMinutes(5));
            return await GetStockPrice(ticker);
        });

        if(stockPriceResponse is null)
        {
            logger.LogWarning("Failed to get stock price for the {Ticker}", ticker);
        }
        else
        {
            logger.LogInformation("Completed getting stock price for {Ticker},{@Stock}",
                ticker,
                stockPriceResponse);
        }
        return stockPriceResponse;
    }

    private async Task<StockPriceResponse?> GetStockPrice(string ticker)
    {
        try
        {
            // Alpha vantage API URL
            string requestUrl = $"query?function=GLOBAL_QUOTE&symbol={ticker}&apikey=" + configuration["Stocks:ApiKey"];

            logger.LogInformation("Request Url:{Url}", requestUrl);

            var response = await httpClient.GetStringAsync(requestUrl);
            logger.LogInformation("Raw response: {Response}", response);

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            // Send Http Request
            var alphaVantageResponse = JsonSerializer.Deserialize<AlphaVantageResponse>(response, options);


            // validate response
            if (alphaVantageResponse?.GlobalQuote == null || string.IsNullOrEmpty(alphaVantageResponse.GlobalQuote.Symbol))
            {
                logger.LogWarning("No data received for ticker:{Ticker}", ticker);
                return null;
            }

            // Extract Stock price
            return new StockPriceResponse(alphaVantageResponse.GlobalQuote.Symbol, alphaVantageResponse.GlobalQuote.Price);
        }
        catch(Exception ex)
        {
            logger.LogError(ex, "Error fetching stock price for {Ticker}", ticker);
            return null;
        }
    }
}
