using Warehouse.Core.Model;
using Warehouse.Tests.Infrastructure.Fixtures;

namespace Warehouse.Tests.Repository;

public class CurrentWarehouseStateReaderTests : IClassFixture<WarshouseInfrastructureFixture>
{
    private readonly WarshouseInfrastructureFixture _fixture;

    public CurrentWarehouseStateReaderTests(WarshouseInfrastructureFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task TestWhenThereAreNoState()
    {
        // Arrange
        var reader = _fixture.CreateCurrentWarehouseStateReader();
        var stateId = Guid.CreateVersion7();
        
        // Act
        var state = await reader.GetWarehouseState(stateId);
        
        // Assert
        
        Assert.Null(state);
    }
    
    [Fact]
    public async Task TestWhenThereAreState()
    {
        // Arrange
        var reader = _fixture.CreateCurrentWarehouseStateReader();
        var writer = _fixture.CreateCurrentWarehouseStateWriter();
        var stateId = Guid.CreateVersion7();
        
        var state = new WarehouseState(stateId, 10);
        
        await writer.AddOrUpdate(state);
        
        // Act
        var stateFromDb = await reader.GetWarehouseState(stateId);
        
        // Assert
        
        Assert.NotNull(stateFromDb);
        Assert.Equal(10u, stateFromDb.AvailableQuantity.Value);
    }
    
    [Fact]
    public async Task TestWhenThereAreStatAndReserve()
    {
        // Arrange
        var reader = _fixture.CreateCurrentWarehouseStateReader();
        var writer = _fixture.CreateCurrentWarehouseStateWriter();
        var stateId = Guid.CreateVersion7();
        
        var state = new WarehouseState(stateId, 10);
        
        await writer.AddOrUpdate(state);
        
        state.Reserve(new ItemSoldQuantity(5));
        
        await writer.AddOrUpdate(state);
        
        // Act
        var stateFromDb = await reader.GetWarehouseState(stateId);
        
        // Assert
        
        Assert.NotNull(stateFromDb);
        Assert.Equal(5u, stateFromDb.AvailableQuantity.Value);
    }
    
    
}