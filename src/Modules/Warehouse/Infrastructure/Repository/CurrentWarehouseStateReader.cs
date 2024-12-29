using Marten;
using Warehouse.Core.Model;
using Warehouse.Core.Repository;

namespace Warehouse.Infrastructure.Repository;

public sealed class CurrentWarehouseStateReader : ICurrentWarehouseStateReader
{
    private readonly IQuerySession _querySession;

    public CurrentWarehouseStateReader(IQuerySession querySession)
    {
        this._querySession = querySession;
    }

    public async Task<CurrentWarehouseState?> GetWarehouseState(ItemId itemId, CancellationToken cancellationToken = default)
    {
        var result = await _querySession.Query<CurrentWarehouseState>()
            .FirstOrDefaultAsync(x => x.ItemId == itemId, cancellationToken);
        
        return result;
    }
}