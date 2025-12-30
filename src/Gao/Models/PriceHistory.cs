namespace Gao.Models;

/// <summary>
/// Represents a price point in the history of an item
/// </summary>
public class PriceHistory
{
    public string ItemId { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public DateTime Timestamp { get; set; }
    public string Source { get; set; } = string.Empty;

    public PriceHistory()
    {
        Timestamp = DateTime.UtcNow;
    }

    public PriceHistory(string itemId, decimal price, string source)
    {
        ItemId = itemId;
        Price = price;
        Source = source;
        Timestamp = DateTime.UtcNow;
    }
}
