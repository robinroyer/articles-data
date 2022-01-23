namespace ef_core_momory_cache_leak;

public class WeatherForecastResponse
{
    public int WeatherForecastCount { get; set; } = 0;
    public List<WeatherForecast> LastForcasts { get; set; } = new();
}
