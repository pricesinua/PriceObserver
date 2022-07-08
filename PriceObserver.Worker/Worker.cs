using CronBackgroundServices;
using Cronos;
using PriceObserver.Persistance;

namespace PriceObserver.Worker;

public class Worker : IRecurringAction
{
    private readonly ILogger<Worker> logger;
    private readonly AppDbContext appDbContext;
    private readonly IConfiguration configuration;

    public Worker(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var scopedServiceProvider = scope.ServiceProvider;

        this.logger = scopedServiceProvider.GetRequiredService<ILogger<Worker>>();
        this.appDbContext = scopedServiceProvider.GetRequiredService<AppDbContext>();
        this.configuration = scopedServiceProvider.GetRequiredService<IConfiguration>();
    }

    public string Cron => configuration.GetValue<string>("ParseSchedule");

    public Task Process(CancellationToken stoppingToken)
    {
        var cronExpression = CronExpression.Parse(Cron, CronFormat.IncludeSeconds);

        var nextTime = cronExpression.GetNextOccurrence(DateTime.UtcNow);

        logger.LogInformation($"Next parse will start at: {nextTime?.ToLocalTime()}");

        return Task.CompletedTask;
    }
}