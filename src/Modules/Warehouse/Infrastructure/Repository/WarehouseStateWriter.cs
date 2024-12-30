using Marten;
using Modular.Ecommerce.Core.Types;
using Warehouse.Core.Model;
using Warehouse.Core.Repository;

namespace Warehouse.Infrastructure.Repository;

public class WarehouseStateWriter : IWarehouseStateWriter
{
    private readonly DocumentStore _documentStore;

    public WarehouseStateWriter(DocumentStore documentStore)
    {
        _documentStore = documentStore;
    }


    public Task<Result<Unit>> Add(WarehouseState state, CancellationToken cancellationToken = default)
    {
        
    }
}