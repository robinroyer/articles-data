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

var app = builder.Build();

app.MapControllers();

using (var scope = app.Services.CreateScope())
using (var context = scope.ServiceProvider.GetService<ef_core_momory_cache_leak.TodoContext>())
{
    context.Database.Migrate();
}

app.Run();
