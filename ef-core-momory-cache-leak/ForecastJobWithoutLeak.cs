namespace ef_core_momory_cache_leak;

public class ForecastJobWithoutLeak
{

    private readonly IServiceProvider _services; 

    public ForecastJobWithoutLeak (IServiceProvider services)
    {
        _services = services;
    }

    public async Task Work()
    {
        var nextForcastDay = 1;
        while(true)
        {
            using var context = _services.GetService<TodoContext>();
            try
            {
                await context.AddAsync(WeatherForecast.GenerateForecast(nextForcastDay++));
                await context.SaveChangesAsync();
            }
            finally
            {
                await Task.Delay(2000);
            }

        }
    }
}
