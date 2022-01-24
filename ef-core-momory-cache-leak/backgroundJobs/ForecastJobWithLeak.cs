using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using Microsoft.EntityFrameworkCore;

namespace ef_core_momory_cache_leak;

[MemoryDiagnoser]
[SimpleJob(RunStrategy.Monitoring, targetCount: 3)]
public class ForecastJobWithLeak : IJob
{
    [Params(100, 500, 1000, 5000, 10000)]
    public int Loop;

    private TodoContext _context;

    public ForecastJobWithLeak (TodoContext context = default)
    {
        _context = context;
    }

    [GlobalSetup]
    public void GlobalSetup()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables();

        var conf = builder.Build();

        var services = new ServiceCollection();
        services.AddDbContext<TodoContext>(
            options => options
                .UseMySql(
                    conf.GetConnectionString("DefaultConnection"),
                    MariaDbServerVersion.LatestSupportedServerVersion));

        var serviceProvider = services.BuildServiceProvider();
        _context = serviceProvider.GetRequiredService<TodoContext>();
    }

    [Benchmark]
    public async Task Work()
    {
        var nextForcastDay = 1;
        while(nextForcastDay < Loop)
        {
            await _context.AddAsync(WeatherForecast.GenerateForecast(nextForcastDay++));
            await _context.SaveChangesAsync();
        }
    }
}
