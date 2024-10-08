using System.Text.Json.Serialization;

namespace Warehouse.Core.Model;

public readonly record struct ItemWarehouseQuantity(int Value)
{
    public static implicit operator ItemWarehouseQuantity(int value) => new ItemWarehouseQuantity(value);
    
    public static implicit operator int(ItemWarehouseQuantity value) => value.Value;
}

public readonly record struct ItemSoldQuantity(int Value)
{
    public static implicit operator ItemSoldQuantity(int value) => new ItemSoldQuantity(value);
    
    public static implicit operator int(ItemSoldQuantity value) => value.Value;
}

public readonly record struct ItemReservedQuantity(int Value)
{
    public static implicit operator ItemReservedQuantity(int value) => new ItemReservedQuantity(value);
    
    public static implicit operator int(ItemReservedQuantity value) => value.Value;
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

public readonly record struct ItemId(Guid Value)
{
    public static ItemId From(Guid id) => new ItemId(id);
    
    public static ItemId New() => new ItemId(Guid.CreateVersion7());
    
    public static implicit operator Guid (ItemId id) => id.Value;
    
    public static implicit operator ItemId (Guid id) => new ItemId(id);
}

public sealed class ItemReserved
{
    public ItemId ItemId { get; init; }
    public ItemSoldQuantity SoldQuantity { get; init; }
}

public abstract class AggregateBase
{
    // For indexing our event streams
    public string Id { get; protected set; }

    // For protecting the state, i.e. conflict prevention
    // The setter is only public for setting up test conditions
    public long Version { get; set; }

    // JsonIgnore - for making sure that it won't be stored in inline projection
    [JsonIgnore] 
    private readonly List<object> _uncommittedEvents = new List<object>();

    // Get the deltas, i.e. events that make up the state, not yet persisted
    public IEnumerable<object> GetUncommittedEvents()
    {
        return _uncommittedEvents;
    }

    // Mark the deltas as persisted.
    public void ClearUncommittedEvents()
    {
        _uncommittedEvents.Clear();
    }

    protected void AddUncommittedEvent(object @event)
    {
        // add the event to the uncommitted list
        _uncommittedEvents.Add(@event);
    }
}

public sealed class WarehouseState(ItemId itemId, ItemAvailability availability) : AggregateBase
{
    public WarehouseState Apply(ItemReserved request)
    {
        if (ItemId != request.ItemId)
        {
            throw new InvalidOperationException("Item id does not match");
        }
        
        var newSoldQ = Availability.SoldQuantity + request.SoldQuantity;
        return this with { Availability = Availability with { SoldQuantity = newSoldQ } };
    }

    public ItemId ItemId { get; init; } = itemId;
    public ItemAvailability Availability { get; init; } = availability;

    public void Deconstruct(out ItemId ItemId, out ItemAvailability Availability)
    {
        ItemId = this.ItemId;
        Availability = this.Availability;
    }
}