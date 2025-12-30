namespace Gao.Models;

/// <summary>
/// Represents an item in the inventory
/// </summary>
public class InventoryItem
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public decimal CurrentPrice { get; set; }
    public int Quantity { get; set; }
    public DateTime LastUpdated { get; set; }
    public string Owner { get; set; } = string.Empty;

    public InventoryItem()
    {
        LastUpdated = DateTime.UtcNow;
    }

    public InventoryItem(string id, string name, decimal price, int quantity, string owner)
    {
        Id = id;
        Name = name;
        CurrentPrice = price;
        Quantity = quantity;
        Owner = owner;
        LastUpdated = DateTime.UtcNow;
    }

    /// <summary>
    /// Creates a deep copy of this inventory item
    /// </summary>
    public InventoryItem Clone()
    {
        return new InventoryItem(Id, Name, CurrentPrice, Quantity, Owner)
        {
            LastUpdated = LastUpdated
        };
    }
}
