using System.Text.Json.Serialization;

namespace StocksApi.Stocks;

public class YahooFinanceResponse
{
    [JsonPropertyName("quoteResponse")]
    public QuoteResponse QuoteResponse { get; set; } = new();
}

public class QuoteResponse
{
    [JsonPropertyName("result")]
    public List<StockData> Result { get; set; } = new();
}

public class StockData
{
    [JsonPropertyName("symbol")]
    public string Symbol { get; set; } = string.Empty;

    [JsonPropertyName("regularMarketPrice")]
    public decimal RegularMarketPrice { get; set; }
}
