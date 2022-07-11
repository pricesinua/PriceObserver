using ApiZakazUa;
using ApiZakazUa.Resources;
using CronBackgroundServices;
using Cronos;
using PriceObserver.Persistance.Data;

namespace PriceObserver.Worker;

public class Worker : WorkerDependencies, IRecurringAction
{
    public string Cron => configuration.GetValue<string>("ParseSchedule");

    private Client client;

    public Worker(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    public async Task Process(CancellationToken stoppingToken)
    {
        LogNextParseTime();

        logger.LogDebug($"Connecting client.");

        client = new ApiZakazUa.Client();

        IReadOnlySet<Store>? stores = null;

        try
        {
            logger.LogDebug($"Fetching stores.");
            stores = await client.GetStoresAsync();
        }
        catch (System.Exception exception)
        {
            logger.LogError($"Failed to fetch stores: {exception.Message}.");
            return;
        }

        logger.LogInformation($"Parse of {stores.Count} stores started.");

        var storeNumber = 0;

        foreach (var store in stores)
        {
            storeNumber++;

            logger.LogTrace($"Processing store: {store.Id} {store.Name}.");

            var categories = await client.GetCategoriesAsync(store.Id);
            if (categories is null) continue;

            foreach (var category in categories)
            {
                logger.LogTrace($"Processing category: {category.Id} {category.Title}.");

                var products = await client.GetProductsAsync(store.Id, category.Id);
                if (products is null) continue;

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