using Warehouse.Core.Model;

namespace Warehouse.Tests.Core.Model;

public class WarehouseStateTests
{
    [Fact]
    public void IncreaseSoldQuantity()
    {
        // Arrange
        var itemId = ItemId.New();
        var warehouseState = new WarehouseState(itemId, new ItemAvailability(10, 0, 0));
        var itemReserved = new ItemReserved { ItemId = itemId, SoldQuantity = 5 };
        
        // Act
        var newState = warehouseState.Apply(itemReserved);
        
        // Assert
        Assert.Equal(5, newState.Availability.SoldQuantity.Value);
    }
}