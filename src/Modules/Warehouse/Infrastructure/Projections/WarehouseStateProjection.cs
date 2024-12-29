using Marten.Events.Aggregation;
using Warehouse.Core.Model;

namespace Warehouse.Infrastructure.Projections;

public class WarehouseStateProjection : SingleStreamProjection<WarehouseState>
{
    public static WarehouseState Create(ItemStateCreated itemStateCreated)
    {
        return new WarehouseState(itemStateCreated);
    }
    
    public WarehouseState Apply(ItemReserved itemReserved, WarehouseState warehouseState)
    {
        return warehouseState.Apply(itemReserved);
    }
}