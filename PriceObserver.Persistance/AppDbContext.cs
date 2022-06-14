namespace PriceObserver.Persistance;

using Microsoft.EntityFrameworkCore;
using PriceObserver.Persistance.Data;

public class AppDbContext : DbContext
{
    public DbSet<ProductEntry> ProductEntries { get; set; }
    public DbSet<PriceStamp> PriceStamps { get; set; }

    public AppDbContext(DbContextOptions options) : base(options) { }
}