using Marten.Events.Aggregation;
using Warehouse.Core.Model;

namespace Warehouse.Infrastructure.Projections;

public sealed class CurrentWarehouseStateProjection : SingleStreamProjection<CurrentWarehouseState>
{
    
}