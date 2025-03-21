
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using StocksApi.Stocks;

namespace StocksApi.Realtime;

internal sealed class StocksFeedUpdater(IServiceScopeFactory serviceScopeFactory,
                            IHubContext<StocksFeedHub, IStockUpdateClient> hubContext,
                            IOptions<StockUpdateOptions> options,
                            ActiveTickerManager activeTickerManager,
                            ILogger<StocksFeedUpdater> logger) : BackgroundService
{
    private readonly Random _random = new();
    private readonly StockUpdateOptions _options = options.Value;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while(!stoppingToken.IsCancellationRequested)
        {
            await UpdateStockPrices();

            await Task.Delay(_options.UpdateInterval, stoppingToken);
        }
    }

    private async Task UpdateStockPrices()
    {
        using IServiceScope scope = serviceScopeFactory.CreateScope();
        StockService stockService = scope.ServiceProvider.GetRequiredService<StockService>();

        foreach(var ticker in activeTickerManager.GetAllTickers())
        {
            StockPriceResponse? currentPrice = await stockService.GetLatestStockPrice(ticker);
            if(currentPrice == null)
            {
                continue;
            }
            decimal newPrice = CalculateNewPrice(currentPrice);
            var update = new StockPriceUpdate(ticker, newPrice);

            //await hubContext.Clients.All.ReceiveStockPriceUpdate(update);

            await hubContext.Clients.Group(ticker).ReceiveStockPriceUpdate(update);
            logger.LogInformation("Updated {Ticker} price to {Price}", ticker, newPrice);
        }
    }

    private decimal CalculateNewPrice(StockPriceResponse currentPrice)
    {
        double change = _options.MaxPercentageChange;
        decimal priceFactor = (decimal)(_random.NextDouble() * change * 2 - change);
        decimal priceChange = currentPrice.Price * priceFactor;
        decimal newPrice = Math.Max(0, currentPrice.Price + priceChange);
        newPrice = Math.Round(newPrice, 2);
        return newPrice;
    }
}
