using CronBackgroundServices;
using PriceObserver.Persistance;

namespace PriceObserver.Worker;

public abstract class WorkerDependencies
{
    protected readonly ILogger<Worker> logger;
    protected readonly AppDbContext appDbContext;
    protected readonly IConfiguration configuration;

    public WorkerDependencies(IServiceProvider serviceProvider)
    {
        var scope = serviceProvider.CreateScope();
        var scopedServiceProvider = scope.ServiceProvider;

        this.logger = scopedServiceProvider.GetRequiredService<ILogger<Worker>>();
        this.appDbContext = scopedServiceProvider.GetRequiredService<AppDbContext>();
        this.configuration = scopedServiceProvider.GetRequiredService<IConfiguration>();
    }
}