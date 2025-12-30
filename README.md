# GAO.CS - Inventory Price Tracker & Cheater Detector

A C# library for tracking inventory item prices and detecting suspicious activities that may indicate cheating or manipulation.

## Features

### 📊 Price Tracking
- **Real-time price monitoring** - Track current prices and price changes for inventory items
- **Price history** - Maintain complete historical records of all price changes
- **Statistical analysis** - Calculate average prices and price volatility over time periods
- **Multi-user support** - Track items across different owners/players

### 🔍 Cheater Detection
- **Price manipulation detection** - Identifies extreme or suspicious price changes
- **Quantity anomaly detection** - Flags unrealistic inventory quantities that may indicate item duplication
- **Rapid transaction monitoring** - Detects unusual transaction patterns
- **Severity scoring** - Assigns risk scores to suspicious activities
- **Comprehensive reporting** - Get detailed reports on all suspicious activities

## Quick Start

### Building the Project

```bash
dotnet build Gao.sln
```

### Running the Demo

```bash
dotnet run --project src/Gao.Demo/Gao.Demo.csproj
```

## Usage Example

```csharp
using Gao.Models;
using Gao.Services;

// Initialize services
var priceTracker = new PriceTrackerService();
var cheaterDetector = new CheaterDetectorService(priceTracker);

// Add an item to inventory
var item = new InventoryItem("ITEM001", "Legendary Sword", 1000m, 1, "Player1");
priceTracker.UpdateItem(item);

// Check for suspicious activity
var activities = cheaterDetector.AnalyzeItemUpdate(item);

// Update item price
item.CurrentPrice = 1500m;
priceTracker.UpdateItem(item);
activities = cheaterDetector.AnalyzeItemUpdate(item);

// Get price history
var history = priceTracker.GetPriceHistory("ITEM001");

// Get all suspicious activities
var allActivities = cheaterDetector.GetAllActivities();
```

## Detection Thresholds

The cheater detector uses the following thresholds (configurable in source):

- **Price Manipulation**: >50% price change
- **Rapid Transactions**: >10 transactions in 60 seconds
- **Unrealistic Quantity**: >100,000 units

## Project Structure

```
gao.cs/
├── src/
│   ├── Gao/                    # Core library
│   │   ├── Models/             # Data models
│   │   │   ├── InventoryItem.cs
│   │   │   ├── PriceHistory.cs
│   │   │   └── SuspiciousActivity.cs
│   │   └── Services/           # Business logic
│   │       ├── PriceTrackerService.cs
│   │       └── CheaterDetectorService.cs
│   └── Gao.Demo/              # Demo application
│       └── Program.cs
├── Gao.sln                    # Solution file
└── README.md
```

## API Reference

### PriceTrackerService

- `UpdateItem(InventoryItem item)` - Add or update an item
- `GetItem(string itemId)` - Retrieve a specific item
- `GetAllItems()` - Get all inventory items
- `GetPriceHistory(string itemId)` - Get price history for an item
- `GetAveragePrice(string itemId, TimeSpan period)` - Calculate average price
- `GetPriceVolatility(string itemId, TimeSpan period)` - Calculate price volatility
- `RemoveItem(string itemId)` - Remove an item from inventory

### CheaterDetectorService

- `AnalyzeItemUpdate(InventoryItem item)` - Analyze an item for suspicious patterns
- `DetectRapidTransactions(string userId, List<InventoryItem> items)` - Check for rapid transactions
- `GetAllActivities()` - Get all detected suspicious activities
- `GetUserActivities(string userId)` - Get activities for a specific user
- `GetHighSeverityActivities(decimal threshold)` - Get high-risk activities
- `ClearOldActivities(TimeSpan maxAge)` - Clean up old activity records

## Requirements

- .NET 8.0 or higher

## License

MIT License