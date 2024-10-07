using Warehouse.Core.Model;

namespace Warehouse.Core.Repository;

public interface IWarehouseStorage
{
    Task<CurrentWarehouseState?> GetWarehouseState(ItemId itemId, CancellationToken cancellationToken = default);
}