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
    public async Task TestWhenAreNoState()
    {
        // Arrange
        var reader = _fixture.CreateCurrentWarehouseStateReader();
        var stateId = Guid.CreateVersion7();
        
        // Act
        var state = await reader.GetWarehouseState(stateId);
        
        // Assert
        
        Assert.Null(state);

    }
}