namespace StocksApi.Realtime;

public interface IStockUpdateClient
{
    Task ReceiveStockPriceUpdate(StockPriceUpdate update);
}

