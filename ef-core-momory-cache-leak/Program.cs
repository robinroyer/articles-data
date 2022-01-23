using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder
    .Services
    .AddDbContext<ef_core_momory_cache_leak.TodoContext>(
        options => options
            .UseMySql(
                builder
                    .Configuration
                    .GetConnectionString("DefaultConnection"),
                MariaDbServerVersion.LatestSupportedServerVersion));

builder.Services.AddScoped<ef_core_momory_cache_leak.ForecastJobWithLeak>();
builder.Services.AddScoped<ef_core_momory_cache_leak.ForecastJobWithoutLeak>();

var app = builder.Build();

app.MapControllers();

using (var scope = app.Services.CreateScope())
using (var context = scope.ServiceProvider.GetService<ef_core_momory_cache_leak.TodoContext>())
{
    context.Database.Migrate();
}


using var jobScope = app.Services.CreateScope();
switch (builder.Configuration.GetValue<string>("backgroundJob"))
{
    case "leak":
        var jobWithLeak = jobScope.ServiceProvider.GetRequiredService<ef_core_momory_cache_leak.ForecastJobWithLeak>();
        jobWithLeak.Work(); // not awaited
        break;
    case "no-leak":
        var jobWithoutLeak = jobScope.ServiceProvider.GetRequiredService<ef_core_momory_cache_leak.ForecastJobWithoutLeak>();
        jobWithoutLeak.Work(); // not awaited
        break;
    default:
        break;
}

app.Run();
