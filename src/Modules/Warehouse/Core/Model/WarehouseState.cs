using System.Text.Json.Serialization;

namespace Warehouse.Core.Model;

public readonly record struct ItemWarehouseQuantity(int Value)
{
    public static implicit operator ItemWarehouseQuantity(int value) => new ItemWarehouseQuantity(value);
    
    public static implicit operator int(ItemWarehouseQuantity value) => value.Value;
    
    public static readonly ItemWarehouseQuantity Zero = new ItemWarehouseQuantity(0);
}

public readonly record struct ItemSoldQuantity(int Value)
{
    public static implicit operator ItemSoldQuantity(int value) => new ItemSoldQuantity(value);
    
    public static implicit operator int(ItemSoldQuantity value) => value.Value;
    
    public static readonly ItemSoldQuantity Zero = new ItemSoldQuantity(0);
}

public readonly record struct ItemReservedQuantity(int Value)
{
    public static implicit operator ItemReservedQuantity(int value) => new ItemReservedQuantity(value);
    
    public static implicit operator int(ItemReservedQuantity value) => value.Value;
    
    public static readonly ItemReservedQuantity Zero = new ItemReservedQuantity(0);
}

public readonly struct AvailableQuantity(uint Value)
{
    public static readonly AvailableQuantity Zero = new AvailableQuantity(0u);
}

public sealed record ItemAvailability(
    ItemWarehouseQuantity WarehouseQuantity,
    ItemSoldQuantity SoldQuantity,
    ItemReservedQuantity ReservedQuantity)
{
    public AvailableQuantity GetAvailableQuantity()
    {
        var available = WarehouseQuantity.Value - SoldQuantity.Value - ReservedQuantity.Value;
        if (available < 0)
        {
            return AvailableQuantity.Zero;
        }
        return new AvailableQuantity((uint)available);
    }
}

public sealed class ItemReserved
{
    public ItemId ItemId { get; init; }
    public ItemSoldQuantity SoldQuantity { get; init; }
}

public sealed class ItemStateCreated
{
    public ItemId ItemId { get; init; }
    public ItemWarehouseQuantity WarehouseQuantity { get; init; }
}


public sealed class WarehouseState(ItemId itemId, ItemAvailability availability)
{
    internal WarehouseState(ItemStateCreated request) : this(request.ItemId, new ItemAvailability(request.WarehouseQuantity, ItemSoldQuantity.Zero, ItemReservedQuantity.Zero))
    {
        
    }
    public WarehouseState Apply(ItemReserved request)
    {
        if (ItemId != request.ItemId)
        {
            throw new InvalidOperationException("Item id does not match");
        }
        
        var newSoldQ = Availability.SoldQuantity + request.SoldQuantity;
        return new WarehouseState(ItemId, Availability with { SoldQuantity = newSoldQ });
    }

    public ItemId ItemId { get; init; } = itemId;
    public ItemAvailability Availability { get; init; } = availability;
    
}