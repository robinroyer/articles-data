using Microsoft.AspNetCore.Mvc;

namespace ef_core_momory_cache_leak.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger;
    private readonly TodoContext _context;

    public WeatherForecastController(
        TodoContext context,
        ILogger<WeatherForecastController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public WeatherForecastResponse Get()
    {
        var newForecasts = Enumerable
            .Range(1, 5)
            .Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();

        _context.AddRange(newForecasts);
        _context.SaveChanges();

        return new () {
            LastForcasts = newForecasts.ToList(),
            WeatherForecastCount = _context.Forecasts.Count(),
        };
    }
}
