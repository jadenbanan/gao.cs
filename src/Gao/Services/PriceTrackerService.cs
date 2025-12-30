using Gao.Models;

namespace Gao.Services;

/// <summary>
/// Service for tracking inventory item prices and price history
/// </summary>
public class PriceTrackerService
{
    private readonly Dictionary<string, InventoryItem> _inventory = new();
    private readonly List<PriceHistory> _priceHistory = new();
    private readonly object _lock = new();

    /// <summary>
    /// Adds or updates an item in the inventory
    /// </summary>
    public void UpdateItem(InventoryItem item)
    {
        lock (_lock)
        {
            if (_inventory.TryGetValue(item.Id, out var existingItem))
            {
                // Track price change
                if (existingItem.CurrentPrice != item.CurrentPrice)
                {
                    _priceHistory.Add(new PriceHistory(item.Id, item.CurrentPrice, item.Owner));
                }
            }
            else
            {
                // New item, record initial price
                _priceHistory.Add(new PriceHistory(item.Id, item.CurrentPrice, item.Owner));
            }

            // Store a copy to avoid reference issues
            var itemCopy = item.Clone();
            itemCopy.LastUpdated = DateTime.UtcNow;
            _inventory[item.Id] = itemCopy;
        }
    }

    /// <summary>
    /// Gets an item by its ID
    /// </summary>
    public InventoryItem? GetItem(string itemId)
    {
        lock (_lock)
        {
            return _inventory.TryGetValue(itemId, out var item) ? item : null;
        }
    }

    /// <summary>
    /// Gets all items in the inventory
    /// </summary>
    public List<InventoryItem> GetAllItems()
    {
        lock (_lock)
        {
            return _inventory.Values.ToList();
        }
    }

    /// <summary>
    /// Gets price history for a specific item
    /// </summary>
    public List<PriceHistory> GetPriceHistory(string itemId)
    {
        lock (_lock)
        {
            return _priceHistory.Where(ph => ph.ItemId == itemId).OrderBy(ph => ph.Timestamp).ToList();
        }
    }

    /// <summary>
    /// Gets price history for all items
    /// </summary>
    public List<PriceHistory> GetAllPriceHistory()
    {
        lock (_lock)
        {
            return _priceHistory.OrderBy(ph => ph.Timestamp).ToList();
        }
    }

    /// <summary>
    /// Calculates average price for an item over a time period
    /// </summary>
    public decimal GetAveragePrice(string itemId, TimeSpan period)
    {
        lock (_lock)
        {
            var cutoffTime = DateTime.UtcNow - period;
            var recentPrices = _priceHistory
                .Where(ph => ph.ItemId == itemId && ph.Timestamp >= cutoffTime)
                .Select(ph => ph.Price)
                .ToList();

            return recentPrices.Any() ? recentPrices.Average() : 0;
        }
    }

    /// <summary>
    /// Gets price volatility (standard deviation) for an item
    /// </summary>
    public decimal GetPriceVolatility(string itemId, TimeSpan period)
    {
        lock (_lock)
        {
            var cutoffTime = DateTime.UtcNow - period;
            var recentPrices = _priceHistory
                .Where(ph => ph.ItemId == itemId && ph.Timestamp >= cutoffTime)
                .Select(ph => (double)ph.Price)
                .ToList();

            if (recentPrices.Count < 2)
                return 0;

            var average = recentPrices.Average();
            var sumOfSquares = recentPrices.Sum(price => Math.Pow(price - average, 2));
            var variance = sumOfSquares / recentPrices.Count;
            return (decimal)Math.Sqrt(variance);
        }
    }

    /// <summary>
    /// Removes an item from the inventory
    /// </summary>
    public bool RemoveItem(string itemId)
    {
        lock (_lock)
        {
            return _inventory.Remove(itemId);
        }
    }
}
