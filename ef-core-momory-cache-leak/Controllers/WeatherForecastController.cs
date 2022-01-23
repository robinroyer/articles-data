using Microsoft.AspNetCore.Mvc;

namespace ef_core_momory_cache_leak.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
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
            .Select(index => WeatherForecast.GenerateForecast(index))
            .ToArray();

        _context.AddRange(newForecasts);
        _context.SaveChanges();

        return new () {
            LastForcasts = newForecasts.ToList(),
            WeatherForecastCount = _context.Forecasts.Count(),
        };
    }
}
