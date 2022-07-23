namespace PriceObserver.Persistance;

using Microsoft.EntityFrameworkCore;
using PriceObserver.Persistance.Data;

public class AppDbContext : DbContext
{
    public DbSet<Pricestamp> Pricestamps { get; set; }

    public AppDbContext(DbContextOptions options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder model)
    {
        model.Entity<Pricestamp>().HasKey(e => e.Id);
    }
}