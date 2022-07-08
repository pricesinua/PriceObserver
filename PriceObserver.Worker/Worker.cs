using PriceObserver.Persistance;

namespace PriceObserver.Worker;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> logger;
    private readonly AppDbContext appDbContext;

    public Worker(ILogger<Worker> logger, AppDbContext appDbContext)
    {
        this.logger = logger;
        this.appDbContext = appDbContext;
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
