using Marten;
using Modular.Ecommerce.Core.Types;
using Warehouse.Core.Model;
using Warehouse.Core.Repository;

namespace Warehouse.Infrastructure.Repository;

internal class WarehouseStateWriter : IWarehouseStateWriter
{
    private readonly DocumentStore _documentStore;

    public WarehouseStateWriter(DocumentStore documentStore)
    {
        _documentStore = documentStore;
    }


    public Task<Result<Unit>> Add(WarehouseState state, CancellationToken cancellationToken = default)
    {
        throw new System.NotImplementedException();
    }

    public Task<Result<Unit>> Update(WarehouseState state, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}