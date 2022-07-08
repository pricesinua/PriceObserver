using Microsoft.EntityFrameworkCore;
using PriceObserver.Persistance;
using PriceObserver.Worker;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((host, services) =>
    {
        var connectionString = host.Configuration.GetValue<string>("ConnectionString");
        services.AddDbContextPool<AppDbContext>(options => {
            options.UseSqlite(connectionString);
        });
        services.AddHostedService<Worker>();
    })
    .Build();

using (var scope = host.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

await host.RunAsync();
