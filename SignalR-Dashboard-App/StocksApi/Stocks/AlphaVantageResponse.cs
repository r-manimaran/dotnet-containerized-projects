namespace StocksApi.Stocks;

using System.Text.Json;
using System.Text.Json.Serialization;

public class AlphaVantageResponse
{
    [JsonPropertyName("Global Quote")]
    public GlobalQuote GlobalQuote { get; set; } = new();
}

public class GlobalQuote
{
    [JsonPropertyName("01. symbol")]
    public string Symbol { get; set; } = string.Empty;

    [JsonPropertyName("05. price")]
    [JsonConverter(typeof(StringToDecimalConverter))]
    public decimal Price { get; set; }
}

public class StringToDecimalConverter : JsonConverter<decimal>
{
    public override decimal Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        string? value = reader.GetString();
        return decimal.TryParse(value, out var result) ? result : 0m;
    }

    public override void Write(Utf8JsonWriter writer, decimal value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}