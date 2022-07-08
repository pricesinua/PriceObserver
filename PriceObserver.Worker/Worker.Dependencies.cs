using CronBackgroundServices;
using PriceObserver.Persistance;

namespace PriceObserver.Worker;

public partial class Worker : IRecurringAction
{
    public Worker(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var scopedServiceProvider = scope.ServiceProvider;

        this.logger = scopedServiceProvider.GetRequiredService<ILogger<Worker>>();
        this.appDbContext = scopedServiceProvider.GetRequiredService<AppDbContext>();
        this.configuration = scopedServiceProvider.GetRequiredService<IConfiguration>();
    }
}