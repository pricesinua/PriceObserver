using CronBackgroundServices;
using Cronos;
using PriceObserver.Persistance;

namespace PriceObserver.Worker;

public partial class Worker : IRecurringAction
{
    private readonly ILogger<Worker> logger;
    private readonly AppDbContext appDbContext;
    private readonly IConfiguration configuration;

    public string Cron => configuration.GetValue<string>("ParseSchedule");

    public Task Process(CancellationToken stoppingToken)
    {
        var cronExpression = CronExpression.Parse(Cron, CronFormat.IncludeSeconds);

        var nextTime = cronExpression.GetNextOccurrence(DateTime.UtcNow);

        logger.LogInformation($"Next parse will start at: {nextTime?.ToLocalTime()}");

        return Task.CompletedTask;
    }
}