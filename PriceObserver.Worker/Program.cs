using Microsoft.EntityFrameworkCore;
using PriceObserver.Persistance;
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
                services.AddHostedService<Worker>();
            })
            .Build();

        using (var scope = host.Services.CreateScope())
        {
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
            var environment = scope.ServiceProvider.GetRequiredService<IHostEnvironment>();

            logger.LogInformation($"Environment mode: {environment.EnvironmentName}");

            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Database.Migrate();
        }

        await host.RunAsync();
    }
}