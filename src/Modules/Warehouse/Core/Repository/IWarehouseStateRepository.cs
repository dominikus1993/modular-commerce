using Modular.Ecommerce.Core.Types;
using Warehouse.Core.Model;

namespace Warehouse.Core.Repository;

public interface IWarehouseStateRepository
{
    Task<Result<Unit>> Save(WarehouseState state, CancellationToken cancellationToken = default);
}