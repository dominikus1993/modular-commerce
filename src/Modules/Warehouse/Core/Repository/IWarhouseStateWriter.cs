using Modular.Ecommerce.Core.Types;
using Warehouse.Core.Model;

namespace Warehouse.Core.Repository;

internal interface IWarehouseStateWriter
{
    Task<Result<Unit>> AddOrUpdate(WarehouseState state, CancellationToken cancellationToken = default);
}