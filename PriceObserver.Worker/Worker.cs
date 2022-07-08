using Cronos;
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
            var scheduleExpressionString = configuration.GetValue<string>("ParseSchedule");
            var cronExpression = CronExpression.Parse(scheduleExpressionString, CronFormat.IncludeSeconds);

            var nextTime = cronExpression.GetNextOccurrence(DateTime.UtcNow);

            logger.LogInformation($"Next parse will start at: {nextTime?.ToLocalTime()}");

            await Task.Delay((TimeSpan)(nextTime?.Subtract(DateTime.UtcNow)), cancellationToken);
        }
    }
}