using Warehouse.Core.Model;

namespace Warehouse.Tests.Core.Model;

public class WarehouseStateTests
{
    [Fact]
    public void IncreaseSoldQuantity()
    {
        // Arrange
        var itemId = Guid.CreateVersion7();
        var warehouseState = new WarehouseState(itemId, new ItemAvailability(10, 0, 0));
        
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
        var warehouseState = new WarehouseState(itemId, new ItemAvailability(10, 0, 0));
        
        // Act
        warehouseState.Reserve(new ItemSoldQuantity(5));
        
        // Assert
        Assert.Equal(5, warehouseState.Availability.SoldQuantity.Value);
        Assert.NotEmpty(warehouseState.GetUncommittedEvents());
    }
}