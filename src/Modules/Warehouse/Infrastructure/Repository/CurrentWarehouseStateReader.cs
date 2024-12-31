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
        await using var session = await _store.LightweightSerializableSessionAsync(cancellationToken);
        WarehouseState? result = await session.Events.FetchLatest<WarehouseState>(itemId, cancellationToken);
        
        if (result is null)
        {
            return null;
        }

        var availableQuantity = result.GetAvailableQuantity();
        return new CurrentWarehouseState(result.Id, availableQuantity);
    }
}