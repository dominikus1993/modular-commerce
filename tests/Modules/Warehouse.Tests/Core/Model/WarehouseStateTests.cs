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
        var itemReserved = new ItemReserved { ItemId = itemId, SoldQuantity = 5 };
        
        // Act
        var newState = warehouseState.Apply(itemReserved);
        
        // Assert
        Assert.Equal(5, newState.Availability.SoldQuantity.Value);
    }
    
    
    [Fact]
    public void CreateNewWarehouseState()
    {
        // Arrange
        var itemId = Guid.CreateVersion7();
        var warehouseState = new WarehouseState(new ItemStateCreated() { ItemId = itemId, WarehouseQuantity = new ItemWarehouseQuantity(10)});
        var itemReserved = new ItemReserved { ItemId = itemId, SoldQuantity = 5 };
        
        // Act
        var newState = warehouseState.Apply(itemReserved);
        
        // Assert
        Assert.Equal(5, newState.Availability.SoldQuantity.Value);
    }
}