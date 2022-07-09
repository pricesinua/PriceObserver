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
        LogNextParseTime();

        var client = new ApiZakazUa.Client();

        var stores = await client.GetStoresAsync();

        logger.LogInformation($"Parse of {stores.Count} stores started.");

        var storeNumber = 0;

        foreach (var store in stores)
        {
            storeNumber++;

            logger.LogTrace($"Processing store: {store.Id} {store.Name}.");
            var categories = await client.GetCategoriesAsync(store.Id);

            foreach (var category in categories)
            {
                logger.LogTrace($"Processing category: {category.Id} {category.Title}.");
                var products = await client.GetProductsAsync(store.Id, category.Id);

                foreach (var product in products)
                {
                    logger.LogTrace($"Processing product: {product.Ean} {product.Slug}.");

                    try
                    {
                        var currency = short.Parse(NodaMoney.Currency.FromCode(product.Currency.ToUpper()).Number);

                        appDbContext.PriceStamps.Add(new PriceStamp()
                        {
                            StoreId = store.Id,
                            ProductEan = product.Ean,
                            Price = product.Price,
                            Currency = currency,
                            Timestamp = DateTime.UtcNow,
                        });
                    }
                    catch (System.Exception)
                    {
                        logger.LogError($"Failed to create pricestamp for product {product.Slug} from store with id {store.Id}.");
                    }
                    finally
                    {
                        logger.LogTrace($"Product {product.Ean} {product.Slug} successfully processed.");
                    }
                }
            }

            await appDbContext.SaveChangesAsync();

            logger.LogInformation($"{storeNumber} of {stores.Count} stores parsed.");
        }

        logger.LogInformation($"All {stores.Count} stores parsed.");
        LogNextParseTime();
    }

    private void LogNextParseTime()
    {
        var cronExpression = CronExpression.Parse(Cron, CronFormat.IncludeSeconds);

        var nextTime = cronExpression.GetNextOccurrence(DateTime.UtcNow);

        logger.LogInformation($"Next parse will start at: {nextTime?.ToLocalTime()}.");
    }
}