using Microsoft.EntityFrameworkCore;
using PriceObserver.Persistance;
using CronBackgroundServices;
using PriceObserver.Worker;

internal class Program
{
    private static async Task Main(string[] args)
    {
        IHost host = Host.CreateDefaultBuilder(args)
            .ConfigureServices((host, services) =>
            {
                var connectionString = host.Configuration.GetValue<string>("ConnectionString");
                services.AddDbContextPool<AppDbContext>(options =>
                {
                    options.UseSqlite(connectionString);
                });
                services.AddRecurrer<Worker>();
            })
            .Build();

        using (var scope = host.Services.CreateScope())
        {
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
            var environment = scope.ServiceProvider.GetRequiredService<IHostEnvironment>();

            logger.LogInformation($"Environment mode: {environment.EnvironmentName}.");

            var appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            appDbContext.Database.EnsureCreated();
            appDbContext.Database.Migrate();
        }

        await host.RunAsync();
    }
}