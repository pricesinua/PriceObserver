namespace PriceObserver.Persistance.Data;

public class PriceStamp
{
    public int ProductId { get; set; }
    public int StoreId { get; set; }
    public int Price { get; set; }
    public short Currency { get; set; }
    public DateTime Timestamp { get; set; }
}