using TestWebApi.Models.Enities;

namespace TestWebApi.Models.Responses;

public class WeatherForecastResponse
{
    public IEnumerable<WeatherForecast> Result { get; set; }
    public int Count { get; set; }
}
