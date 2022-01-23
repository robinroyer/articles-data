namespace ef_core_momory_cache_leak;

public class WeatherForecast
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    public int ForcastNumber { get; set; }
    public DateTime Date { get; set; }

    public int TemperatureC { get; set; }

    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

    public string? Summary { get; set; }

    public static WeatherForecast GenerateForecast(int extraDay)
    {
        return new()
        {
            Date = DateTime.Now.AddDays(extraDay),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)],
        };
    }
}
