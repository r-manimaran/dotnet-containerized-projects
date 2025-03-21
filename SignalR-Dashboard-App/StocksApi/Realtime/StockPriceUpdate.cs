namespace StocksApi.Realtime;

public sealed record StockPriceUpdate(string Ticker, decimal Price);

