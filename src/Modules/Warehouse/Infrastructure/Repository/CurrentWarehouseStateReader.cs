using Marten;
using Warehouse.Core.Model;
using Warehouse.Core.Repository;

namespace Warehouse.Infrastructure.Repository;

public sealed class CurrentWarehouseStateReader : ICurrentWarehouseStateReader
{
    private readonly DocumentStore _store;

    public CurrentWarehouseStateReader(DocumentStore store)
    {
        _store = store;
    }

    public async Task<CurrentWarehouseState?> GetWarehouseState(ItemId itemId, CancellationToken cancellationToken = default)
    {
        await using var session = _store.LightweightSession();
        var result = await session.Query<WarehouseState>()
            .FirstOrDefaultAsync(x => x.ItemId == itemId, cancellationToken);
        
        if (result is null)
        {
            return null;
        }

        var availableQuantity = result.GetAvailableQuantity();
        return new CurrentWarehouseState(result.ItemId, availableQuantity);
    }
}