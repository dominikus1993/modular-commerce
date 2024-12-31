using Modular.Ecommerce.Core.Types;
using Warehouse.Core.Model;

namespace Warehouse.Core.Repository;

internal interface IWarehouseStateWriter
{
    Task<Result<Unit>> Add(WarehouseState state, CancellationToken cancellationToken = default);
    Task<Result<Unit>> Update(WarehouseState state, CancellationToken cancellationToken = default);
}