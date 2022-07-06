using Microsoft.EntityFrameworkCore;
using PriceObserver.Persistance;
using PriceObserver.Worker;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddDbContextPool<AppDbContext>(options => {
            options.UseSqlite("Data Source=appdata.sqlite");
        });
        services.AddHostedService<Worker>();
    })
    .Build();

await host.RunAsync();
