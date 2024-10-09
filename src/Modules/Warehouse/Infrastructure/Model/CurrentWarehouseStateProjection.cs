using Marten.Events.Aggregation;
using Warehouse.Core.Model;

namespace Warehouse.Infrastructure.Model;

public sealed class CurrentWarehouseStateProjection(ItemId itemId, AvailableQuantity availableQuantity) : SingleStreamProjection<WarehouseState>
{

    public ItemId ItemId { get; init; } = itemId;
    public AvailableQuantity AvailableQuantity { get; init; } = availableQuantity;
 
    
    public static CurrentWarehouseState ToCurrentWarehouseState(CurrentWarehouseStateProjection projection)
    {
        return new CurrentWarehouseState(projection.ItemId, projection.AvailableQuantity);
    }
}