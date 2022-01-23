namespace ef_core_momory_cache_leak;

public class WeatherForecast
{
    public int ForcastNumber { get; set; }
    public DateTime Date { get; set; }

    public int TemperatureC { get; set; }

    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

    public string? Summary { get; set; }
}
