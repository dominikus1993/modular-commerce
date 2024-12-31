using Warehouse.Core.Model;

namespace Warehouse.Tests.Core.Model;

public class WarehouseStateTests
{
    [Fact]
    public void IncreaseSoldQuantity()
    {
        // Arrange
        var itemId = Guid.CreateVersion7();
        var warehouseState = new WarehouseState(itemId, 10);
        
        // Act
        warehouseState.Reserve(new ItemSoldQuantity(5));
        
        // Assert
        Assert.Equal(5, warehouseState.Availability.SoldQuantity.Value);
        Assert.NotEmpty(warehouseState.GetUncommittedEvents());
    }
    
    
    [Fact]
    public void CreateNewWarehouseState()
    {
        // Arrange
        var itemId = Guid.CreateVersion7();
        var warehouseState = new WarehouseState(itemId, 10);
        
        // Assert
        Assert.NotEmpty(warehouseState.GetUncommittedEvents());
    }
}