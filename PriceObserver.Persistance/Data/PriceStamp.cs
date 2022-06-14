namespace PriceObserver.Persistance.Data;

public class PriceStamp
{
    public Guid ProductEntryId { get; set; }
    public int Value { get; set; }
    public short CurrencyCode { get; set; }
    public DateTime TimeStamp { get; set; }

    public virtual ProductEntry ProductEntry { get; set; }
}