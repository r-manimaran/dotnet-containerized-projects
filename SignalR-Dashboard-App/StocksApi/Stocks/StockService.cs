using Dapper;
using Npgsql;
using StocksApi.Realtime;
using System.Runtime.InteropServices;

namespace StocksApi.Stocks;

public class StockService(NpgsqlDataSource dataSource,
                          StocksClient stocksClient,
                          ActiveTickerManager activeTickerManager,
                          ILogger<StockService> logger)
{
    public async Task<StockPriceResponse?> GetLatestStockPrice(string ticker)
    {
        try
        {
            StockPriceResponse? dbPrice = await GetLatestPriceFromDatabase(ticker);
            if(dbPrice is not null)
            {
                activeTickerManager.AddTicker(ticker);
                return dbPrice;
            }

            StockPriceResponse? apiPrice = await stocksClient.GetDataForTicker(ticker);
            if(apiPrice == null)
            {
                logger.LogWarning("No data returned from external API for ticker:{ticker}", ticker);
                return null;
            }

            // save the new Price to th database
            await SavePriceToDatabase(apiPrice);

            activeTickerManager.AddTicker(ticker);
            
            return apiPrice;
        }
        catch(Exception ex)
        {
            logger.LogError(ex, "Error occured while fetching stock price for ticker:{Ticker}", ticker);
            throw;
        }
    }   

    private async Task<StockPriceResponse?> GetLatestPriceFromDatabase(string ticker)
    {
        const string sql =
            """
            SELECT ticker, price,timestamp 
            FROM public.stock_prices
            WHERE ticker = @Ticker
            ORDER BY timestamp DESC
            LIMIT 1
            """;
        using NpgsqlConnection connection = await dataSource.OpenConnectionAsync();

        StockPriceRecord? result = await connection.QueryFirstOrDefaultAsync<StockPriceRecord>(sql, new
        {
            Ticker = ticker
        });

        if(result is not null)
        {
            return new StockPriceResponse(result.Ticker, result.Price);
        }

        return null;

    }

    private async Task SavePriceToDatabase(StockPriceResponse apiPrice)
    {
        const string sql =
            """
            INSERT INTO public.stock_prices (ticker, price, timestamp)
            VALUES (@Ticker, @Price, @Timestamp)
            """;
        using NpgsqlConnection connection = await dataSource.OpenConnectionAsync();
        await connection.ExecuteAsync(sql, new
        {
            apiPrice.Ticker,
            apiPrice.Price,
            TimeStamp = DateTime.UtcNow
        });
    }
}
