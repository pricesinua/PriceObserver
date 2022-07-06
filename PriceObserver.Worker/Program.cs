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

using (var scope = host.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

await host.RunAsync();
