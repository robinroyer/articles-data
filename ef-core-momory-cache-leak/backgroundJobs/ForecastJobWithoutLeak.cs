using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using Microsoft.EntityFrameworkCore;

namespace ef_core_momory_cache_leak;

[MemoryDiagnoser]
[SimpleJob(RunStrategy.Monitoring, targetCount: 3, baseline: true)]
public class ForecastJobWithoutLeak : IJob
{
    [Params(100, 500, 1000, 5000, 10000)]
    public int Loop;
    private IServiceProvider _services;

    private IConfigurationRoot _configuration;

    public ForecastJobWithoutLeak (IServiceProvider services = default)
    {
        _services = services;
    }

    [GlobalSetup]
    public void GlobalSetup()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables();

        _configuration = builder.Build();

        var services = new ServiceCollection();
        services.AddDbContext<TodoContext>(
            options => options
                .UseMySql(
                    _configuration.GetConnectionString("DefaultConnection"),
                    MariaDbServerVersion.LatestSupportedServerVersion));

        _services = services.BuildServiceProvider();
    }

    [Benchmark]
    public async Task Work()
    {
        var nextForcastDay = 1;
        while(nextForcastDay < Loop)
        {
            using var scope = _services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<TodoContext>();
            await context.AddAsync(WeatherForecast.GenerateForecast(nextForcastDay++));
            await context.SaveChangesAsync();
        }
    }
}
