namespace ef_core_momory_cache_leak;

public class ForecastJobWithLeak
{

    private readonly TodoContext _context; 

    public ForecastJobWithLeak (TodoContext context)
    {
        _context = context;
    }

    public async Task Work()
    {
        var nextForcastDay = 1;
        while(true)
        {
            try
            {
                await _context.AddAsync(WeatherForecast.GenerateForecast(nextForcastDay++));
                await _context.SaveChangesAsync();
            }
            finally
            {
                await Task.Delay(2000);
            }
        }
    }
}
