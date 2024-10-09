using Warehouse.Core.Model;

namespace Warehouse.Core.Repository;

public interface ICurrentWarehouseStateReader
{
    Task<CurrentWarehouseState?> GetWarehouseState(ItemId itemId, CancellationToken cancellationToken = default);
}