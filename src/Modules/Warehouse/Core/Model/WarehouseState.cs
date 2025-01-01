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

public readonly record struct AvailableQuantity(int Value)
{
    public static readonly AvailableQuantity Zero = new AvailableQuantity(0);
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
        return new AvailableQuantity(available);
    }
}

public sealed class ItemSold
{
    public ItemId ItemId { get; init; }
    public ItemSoldQuantity SoldQuantity { get; init; }
}

public sealed class ItemStateCreated
{
    public ItemId ItemId { get; init; }
    public ItemWarehouseQuantity WarehouseQuantity { get; init; }
}

public abstract class AggregateBase
{
    // For indexing our event streams
    public Guid Id { get; protected set; }

    // For protecting the state, i.e. conflict prevention
    // The setter is only public for setting up test conditions
    public long Version { get; set; }

    // JsonIgnore - for making sure that it won't be stored in inline projection
    [JsonIgnore] 
    private readonly List<object> _uncommittedEvents = [];

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
        ArgumentNullException.ThrowIfNull(@event);
        // add the event to the uncommitted list
        _uncommittedEvents.Add(@event);
    }
}


public sealed class WarehouseState : AggregateBase
{
    public WarehouseState(ItemId id, ItemWarehouseQuantity warehouseQuantity)
    {
        var itemStateCreated = new ItemStateCreated
        {
            ItemId = id,
            WarehouseQuantity = warehouseQuantity,
        };
        
        AddUncommittedEvent(itemStateCreated);
        Apply(itemStateCreated);
    }
    
    [JsonConstructor]
    public WarehouseState(ItemId id, ItemAvailability availability)
    {
        Id = id;
        Availability = availability;
    }
    
    public static WarehouseState Create(ItemStateCreated created) => new WarehouseState(created.ItemId, new ItemAvailability(created.WarehouseQuantity, 0, 0));
    
    public void Reserve(ItemSoldQuantity soldQuantity)
    {
        var itemReserved = new ItemSold
        {
            ItemId = Id,
            SoldQuantity = soldQuantity,
        };
        
        AddUncommittedEvent(itemReserved);
        Apply(itemReserved);
    }
    
    private void Apply(ItemSold request)
    {
        if (Id != request.ItemId)
        {
            throw new InvalidOperationException("Item id does not match");
        }
        
        var newSoldQ = Availability.SoldQuantity + request.SoldQuantity;
        Availability = Availability with { SoldQuantity = newSoldQ };
        Version++;
    }
    
    private void Apply(ItemStateCreated request)
    {
        Id = request.ItemId;
        Availability = new ItemAvailability(request.WarehouseQuantity, 0, 0);
        Version++;
    }
    
    public ItemAvailability Availability { get; private set; }
    
    public AvailableQuantity GetAvailableQuantity() => Availability.GetAvailableQuantity();
    
}