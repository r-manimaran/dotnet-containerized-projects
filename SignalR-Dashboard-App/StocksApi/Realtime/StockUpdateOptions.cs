
namespace StocksApi.Realtime;

internal sealed class StockUpdateOptions
{
    // how often the Ticker prices needs to be updated
    public TimeSpan UpdateInterval { get; set; } = TimeSpan.FromSeconds(5);

    public double MaxPercentageChange { get; set; } = 0.02; // 2%
}