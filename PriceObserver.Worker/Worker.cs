using CronBackgroundServices;
using Cronos;
using PriceObserver.Persistance;
using PriceObserver.Persistance.Data;

namespace PriceObserver.Worker;

public partial class Worker : IRecurringAction
{
    private readonly ILogger<Worker> logger;
    private readonly AppDbContext appDbContext;
    private readonly IConfiguration configuration;

    public string Cron => configuration.GetValue<string>("ParseSchedule");

    public async Task Process(CancellationToken stoppingToken)
    {
        var client = new ApiZakazUa.Client();

        var stores = await client.GetStoresAsync();

        foreach (var store in stores)
        {
            logger.LogTrace($"Processing store: {store.Id} {store.Name}.");
            var categories = await client.GetCategoriesAsync(store.Id);

            foreach (var category in categories)
            {
                logger.LogTrace($"Processing category: {category.Id} {category.Title}.");
                var products = await client.GetProductsAsync(store.Id, category.Id);

                foreach (var product in products)
                {
                    logger.LogTrace($"Processing product: {product.Ean} {product.Title}.");
                    appDbContext.PriceStamps.Add(new PriceStamp()
                    {
                        StoreId = store.Id,
                        ProductId = product.Ean.GetHashCode(),
                        Price = product.Price,
                        Currency = short.Parse(NodaMoney.Currency.FromCode(product.Currency.ToUpper()).Number),
                        Timestamp = DateTime.UtcNow,
                    });
                }
            }

            await appDbContext.SaveChangesAsync();
        }

        var cronExpression = CronExpression.Parse(Cron, CronFormat.IncludeSeconds);

        var nextTime = cronExpression.GetNextOccurrence(DateTime.UtcNow);

        logger.LogInformation($"Next parse will start at: {nextTime?.ToLocalTime()}.");
    }
}