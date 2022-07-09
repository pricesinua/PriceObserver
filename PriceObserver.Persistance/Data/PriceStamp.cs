using System.Numerics;

namespace PriceObserver.Persistance.Data;

public class PriceStamp
{
    public long Id { get; set; }
    public string ProductEan { get; set; }
    public int StoreId { get; set; }
    public int Price { get; set; }
    public short Currency { get; set; }
    public DateTime Timestamp { get; set; }
}