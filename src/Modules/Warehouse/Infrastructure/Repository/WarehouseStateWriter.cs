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


    public async Task<Result<Unit>> AddOrUpdate(WarehouseState state, CancellationToken cancellationToken = default)
    {
        var events = state.GetUncommittedEvents().ToArray();
        
        if (events.Length == 0)
        {
            return Result.UnitResult;
        }
        
        await using var session = await _documentStore.LightweightSerializableSessionAsync(cancellationToken);
        
        foreach (var @event in events)
        {
            session.Events.Append(state.Id, @event);
        }
        
        await session.SaveChangesAsync(cancellationToken);
        
        state.ClearUncommittedEvents();
        
        return Result.UnitResult;
    }
    
}