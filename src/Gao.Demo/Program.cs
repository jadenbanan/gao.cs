using Gao.Models;
using Gao.Services;

namespace Gao.Demo;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== GAO - Inventory Price Tracker & Cheater Detector Demo ===\n");

        // Initialize services
        var priceTracker = new PriceTrackerService();
        var cheaterDetector = new CheaterDetectorService(priceTracker);

        Console.WriteLine("1. Adding legitimate items to inventory...\n");
        
        // Normal inventory operations
        var item1 = new InventoryItem("ITEM001", "Legendary Sword", 1000m, 1, "Player1");
        priceTracker.UpdateItem(item1);
        cheaterDetector.AnalyzeItemUpdate(item1);
        Console.WriteLine($"✓ Added: {item1.Name} - ${item1.CurrentPrice} (Qty: {item1.Quantity})");

        var item2 = new InventoryItem("ITEM002", "Health Potion", 50m, 10, "Player2");
        priceTracker.UpdateItem(item2);
        cheaterDetector.AnalyzeItemUpdate(item2);
        Console.WriteLine($"✓ Added: {item2.Name} - ${item2.CurrentPrice} (Qty: {item2.Quantity})");

        var item3 = new InventoryItem("ITEM003", "Magic Staff", 750m, 1, "Player3");
        priceTracker.UpdateItem(item3);
        cheaterDetector.AnalyzeItemUpdate(item3);
        Console.WriteLine($"✓ Added: {item3.Name} - ${item3.CurrentPrice} (Qty: {item3.Quantity})");

        // Simulate normal price update
        Console.WriteLine("\n2. Normal price update...\n");
        item1.CurrentPrice = 1100m;
        priceTracker.UpdateItem(item1);
        var activities = cheaterDetector.AnalyzeItemUpdate(item1);
        Console.WriteLine($"✓ Updated: {item1.Name} - ${item1.CurrentPrice}");
        Console.WriteLine($"  Suspicious activities detected: {activities.Count}");

        // Price tracking features
        Console.WriteLine("\n3. Price Tracking Statistics...\n");
        var history = priceTracker.GetPriceHistory("ITEM001");
        Console.WriteLine($"Price history for {item1.Name}:");
        foreach (var h in history)
        {
            Console.WriteLine($"  • ${h.Price} at {h.Timestamp:yyyy-MM-dd HH:mm:ss}");
        }
        
        var avgPrice = priceTracker.GetAveragePrice("ITEM001", TimeSpan.FromHours(1));
        Console.WriteLine($"Average price (last hour): ${avgPrice}");

        // Suspicious activity #1: Extreme price manipulation
        Console.WriteLine("\n4. Testing Cheater Detection - Price Manipulation...\n");
        item2.CurrentPrice = 500m; // 10x price increase!
        priceTracker.UpdateItem(item2);
        activities = cheaterDetector.AnalyzeItemUpdate(item2);
        Console.WriteLine($"⚠ Price changed from $50 to $500");
        Console.WriteLine($"Suspicious activities detected: {activities.Count}");
        if (activities.Count > 0)
        {
            foreach (var activity in activities)
            {
                Console.WriteLine($"  • Type: {activity.ActivityType}");
                Console.WriteLine($"  • User: {activity.UserId}");
                Console.WriteLine($"  • Severity: {activity.SeverityScore:F2}");
                Console.WriteLine($"  • Description: {activity.Description}");
            }
        }

        // Suspicious activity #2: Unrealistic quantity
        Console.WriteLine("\n5. Testing Cheater Detection - Item Duplication...\n");
        var suspiciousItem = new InventoryItem("ITEM004", "Diamond", 5000m, 999999, "SuspiciousPlayer");
        priceTracker.UpdateItem(suspiciousItem);
        activities = cheaterDetector.AnalyzeItemUpdate(suspiciousItem);
        Console.WriteLine($"⚠ Player has {suspiciousItem.Quantity} diamonds!");
        Console.WriteLine($"Suspicious activities detected: {activities.Count}");
        if (activities.Count > 0)
        {
            foreach (var activity in activities)
            {
                Console.WriteLine($"  • Type: {activity.ActivityType}");
                Console.WriteLine($"  • User: {activity.UserId}");
                Console.WriteLine($"  • Severity: {activity.SeverityScore:F2}");
                Console.WriteLine($"  • Description: {activity.Description}");
            }
        }

        // Suspicious activity #3: Rapid transactions
        Console.WriteLine("\n6. Testing Cheater Detection - Rapid Transactions...\n");
        var rapidItems = new List<InventoryItem>();
        for (int i = 0; i < 15; i++)
        {
            var rapidItem = new InventoryItem($"RAPID{i:D3}", $"Item{i}", 100m, 1, "RapidPlayer");
            priceTracker.UpdateItem(rapidItem);
            rapidItems.Add(rapidItem);
        }
        var rapidActivity = cheaterDetector.DetectRapidTransactions("RapidPlayer", rapidItems);
        Console.WriteLine($"⚠ Player made 15 transactions in rapid succession");
        if (rapidActivity != null)
        {
            Console.WriteLine($"Suspicious activity detected:");
            Console.WriteLine($"  • Type: {rapidActivity.ActivityType}");
            Console.WriteLine($"  • User: {rapidActivity.UserId}");
            Console.WriteLine($"  • Severity: {rapidActivity.SeverityScore:F2}");
            Console.WriteLine($"  • Description: {rapidActivity.Description}");
        }

        // Summary report
        Console.WriteLine("\n7. Security Summary Report...\n");
        var allActivities = cheaterDetector.GetAllActivities();
        Console.WriteLine($"Total suspicious activities detected: {allActivities.Count}");
        
        var highSeverity = cheaterDetector.GetHighSeverityActivities(50m);
        Console.WriteLine($"High severity activities (>50): {highSeverity.Count}");

        Console.WriteLine("\nAll Suspicious Activities:");
        foreach (var activity in allActivities)
        {
            Console.WriteLine($"\n  [{activity.DetectedAt:yyyy-MM-dd HH:mm:ss}] {activity.ActivityType}");
            Console.WriteLine($"  User: {activity.UserId}");
            Console.WriteLine($"  Severity: {activity.SeverityScore:F2}");
            Console.WriteLine($"  {activity.Description}");
        }

        Console.WriteLine("\n8. Inventory Overview...\n");
        var allItems = priceTracker.GetAllItems();
        Console.WriteLine($"Total items in inventory: {allItems.Count}");
        Console.WriteLine("\nItems:");
        foreach (var item in allItems.OrderBy(i => i.Name))
        {
            Console.WriteLine($"  • {item.Name,-20} ${item.CurrentPrice,-10:F2} Qty: {item.Quantity,-8} Owner: {item.Owner}");
        }

        Console.WriteLine("\n=== Demo Complete ===");
    }
}
