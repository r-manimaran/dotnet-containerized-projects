using static Aspire.FrontEnd.Components.Pages.Weather;

namespace Aspire.FrontEnd;

public class WeatherApiClient(HttpClient httpClient)
{
    public async Task<WeatherForecast[]> GetWeatherForecastAsync(int maxItems =10, CancellationToken cancellationToken = default)
    {
        List<WeatherForecast>? forecasts = null;

        await foreach (var forecast in httpClient.GetFromJsonAsAsyncEnumerable<WeatherForecast>("/WeatherForecast", cancellationToken))
        {
           
            if (forecasts?.Count >= maxItems)
            {
                break;
            }
            if(forecasts is not null)
            {
                forecasts ??= [];
                forecasts.Add(forecast);
            }
        }
        return  forecasts?.ToArray() ?? [];
    }
}
