using Modular.Ecommerce.Core.Types;
using Warehouse.Core.Model;

namespace Warehouse.Core.Repository;

internal interface IWarehouseStateRepository
{
    Task<Result<Unit>> Save(WarehouseState state, CancellationToken cancellationToken = default);
}