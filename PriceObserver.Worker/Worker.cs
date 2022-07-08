using PriceObserver.Persistance;

namespace PriceObserver.Worker;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> logger;
    private readonly AppDbContext appDbContext;
    private readonly IConfiguration configuration;

    public Worker(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var scopedServiceProvider = scope.ServiceProvider;

        this.logger = scopedServiceProvider.GetService<ILogger<Worker>>();
        this.appDbContext = scopedServiceProvider.GetService<AppDbContext>();
        this.configuration = scopedServiceProvider.GetService<IConfiguration>();
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            await Task.Delay(1000, cancellationToken);
        }
    }
}