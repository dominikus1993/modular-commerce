using Marten.Events.Projections;

using Warehouse.Core.Model;

namespace Warehouse.Infrastructure.Projections;

public class CurrentWarehouseStateProjection : EventProjection
{
    public CurrentWarehouseStateProjection()
    {
        base.Project<ItemStateCreated>((evt, ops) =>
        {
            ops.Store(new CurrentWarehouseState(evt.ItemId, new ItemAvailability(evt.WarehouseQuantity, 0, 0)));
        });
        
        base.ProjectAsync<ItemSold>(async (evt, ops) =>
        {

            var state = await ops.LoadAsync<CurrentWarehouseState>(evt.ItemId);
            if (state is null)
            {
                throw new InvalidOperationException("State not found");
            }
            state = state with
            {
                Availability = state.Availability with
                {
                    ReservedQuantity = state.Availability.SoldQuantity + evt.SoldQuantity
                }
            };
            
            ops.Update<CurrentWarehouseState>(state);
        });
    }
}