using Gao.Models;

namespace Gao.Services;

/// <summary>
/// Service for detecting suspicious activities and potential cheating
/// </summary>
public class CheaterDetectorService
{
    private readonly PriceTrackerService _priceTracker;
    private readonly List<SuspiciousActivity> _detectedActivities = new();
    private readonly object _lock = new();

    // Detection thresholds
    private const decimal EXTREME_PRICE_CHANGE_THRESHOLD = 0.5m; // 50% change
    private const int RAPID_TRANSACTION_THRESHOLD = 10; // transactions
    private const int RAPID_TRANSACTION_WINDOW_SECONDS = 60;
    private const decimal HIGH_VOLUME_MULTIPLIER = 100m;

    public CheaterDetectorService(PriceTrackerService priceTracker)
    {
        _priceTracker = priceTracker;
    }

    /// <summary>
    /// Analyzes an item update for suspicious patterns
    /// </summary>
    public List<SuspiciousActivity> AnalyzeItemUpdate(InventoryItem item)
    {
        var activities = new List<SuspiciousActivity>();

        // Check for extreme price manipulation
        var priceActivity = DetectPriceManipulation(item);
        if (priceActivity != null)
            activities.Add(priceActivity);

        // Check for unrealistic quantity
        var quantityActivity = DetectUnrealisticQuantity(item);
        if (quantityActivity != null)
            activities.Add(quantityActivity);

        // Store detected activities
        lock (_lock)
        {
            _detectedActivities.AddRange(activities);
        }

        return activities;
    }

    /// <summary>
    /// Detects price manipulation patterns
    /// </summary>
    private SuspiciousActivity? DetectPriceManipulation(InventoryItem item)
    {
        var history = _priceTracker.GetPriceHistory(item.Id);
        if (history.Count < 2)
            return null;

        var recentHistory = history.TakeLast(2).ToList();
        var previousPrice = recentHistory[0].Price;
        var currentPrice = item.CurrentPrice;

        if (previousPrice == 0)
            return null;

        var priceChange = Math.Abs((currentPrice - previousPrice) / previousPrice);

        if (priceChange > EXTREME_PRICE_CHANGE_THRESHOLD)
        {
            return new SuspiciousActivity(
                item.Owner,
                "PriceManipulation",
                $"Extreme price change detected: {previousPrice:C} → {currentPrice:C} ({priceChange:P})",
                priceChange * 100
            )
            {
                Metadata = new Dictionary<string, object>
                {
                    ["ItemId"] = item.Id,
                    ["ItemName"] = item.Name,
                    ["PreviousPrice"] = previousPrice,
                    ["CurrentPrice"] = currentPrice,
                    ["ChangePercent"] = priceChange
                }
            };
        }

        return null;
    }

    /// <summary>
    /// Detects unrealistic inventory quantities
    /// </summary>
    private SuspiciousActivity? DetectUnrealisticQuantity(InventoryItem item)
    {
        // Check for extremely high quantities that might indicate duplication
        if (item.Quantity > HIGH_VOLUME_MULTIPLIER * 1000)
        {
            return new SuspiciousActivity(
                item.Owner,
                "UnrealisticQuantity",
                $"Extremely high quantity detected: {item.Quantity} units of {item.Name}",
                Math.Min(100, item.Quantity / 1000m)
            )
            {
                Metadata = new Dictionary<string, object>
                {
                    ["ItemId"] = item.Id,
                    ["ItemName"] = item.Name,
                    ["Quantity"] = item.Quantity
                }
            };
        }

        return null;
    }

    /// <summary>
    /// Analyzes user activity patterns for rapid/suspicious transactions
    /// </summary>
    public SuspiciousActivity? DetectRapidTransactions(string userId, List<InventoryItem> recentItems)
    {
        var cutoffTime = DateTime.UtcNow.AddSeconds(-RAPID_TRANSACTION_WINDOW_SECONDS);
        var rapidTransactions = recentItems
            .Where(item => item.Owner == userId && item.LastUpdated >= cutoffTime)
            .Count();

        if (rapidTransactions > RAPID_TRANSACTION_THRESHOLD)
        {
            var activity = new SuspiciousActivity(
                userId,
                "RapidTransactions",
                $"Unusual transaction rate: {rapidTransactions} transactions in {RAPID_TRANSACTION_WINDOW_SECONDS} seconds",
                rapidTransactions * 5m
            )
            {
                Metadata = new Dictionary<string, object>
                {
                    ["TransactionCount"] = rapidTransactions,
                    ["TimeWindowSeconds"] = RAPID_TRANSACTION_WINDOW_SECONDS
                }
            };

            lock (_lock)
            {
                _detectedActivities.Add(activity);
            }

            return activity;
        }

        return null;
    }

    /// <summary>
    /// Gets all detected suspicious activities
    /// </summary>
    public List<SuspiciousActivity> GetAllActivities()
    {
        lock (_lock)
        {
            return _detectedActivities.OrderByDescending(a => a.DetectedAt).ToList();
        }
    }

    /// <summary>
    /// Gets suspicious activities for a specific user
    /// </summary>
    public List<SuspiciousActivity> GetUserActivities(string userId)
    {
        lock (_lock)
        {
            return _detectedActivities
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.DetectedAt)
                .ToList();
        }
    }

    /// <summary>
    /// Gets high-severity activities (score above threshold)
    /// </summary>
    public List<SuspiciousActivity> GetHighSeverityActivities(decimal severityThreshold = 50m)
    {
        lock (_lock)
        {
            return _detectedActivities
                .Where(a => a.SeverityScore >= severityThreshold)
                .OrderByDescending(a => a.SeverityScore)
                .ToList();
        }
    }

    /// <summary>
    /// Clears old activities beyond a certain age
    /// </summary>
    public int ClearOldActivities(TimeSpan maxAge)
    {
        lock (_lock)
        {
            var cutoffTime = DateTime.UtcNow - maxAge;
            var oldActivities = _detectedActivities.Where(a => a.DetectedAt < cutoffTime).ToList();
            foreach (var activity in oldActivities)
            {
                _detectedActivities.Remove(activity);
            }
            return oldActivities.Count;
        }
    }
}
