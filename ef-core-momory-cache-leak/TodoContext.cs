using Microsoft.EntityFrameworkCore;

namespace ef_core_momory_cache_leak;

public class TodoContext : DbContext
{

    public TodoContext(DbContextOptions<TodoContext>options)
        : base(options)
    {}

    public virtual DbSet<WeatherForecast> Forecasts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var weatherForecastEntity = modelBuilder.Entity<WeatherForecast>();
        weatherForecastEntity.HasKey(f => f.ForcastNumber);
        weatherForecastEntity.ToTable("Forecasts");
    }
}
