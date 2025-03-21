namespace StocksApi.Stocks;

internal sealed record StockPriceRecord (string Ticker, decimal Price, DateTime TimeStamp);