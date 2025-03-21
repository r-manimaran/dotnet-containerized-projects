using StocksApi.Stocks;

namespace StocksApi.Endpoints;

public static class StocksEndpoints
{
    public static void MapStocksEndpoints(this IEndpointRouteBuilder route)
    {
        var group = route.MapGroup("/api/Stocks/").WithOpenApi().WithTags("Stocks");

        group.MapGet("{ticker}", async (string ticker, StockService stockService) =>
        {
            StockPriceResponse? result = await stockService.GetLatestStockPrice(ticker);

            return result is null ?
                   Results.NotFound($"No stock data available for {ticker}:") :
                   Results.Ok(result);

        }).WithName("GetLatestStockPrice")
        .WithOpenApi();
    }
}
