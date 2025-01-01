namespace Warehouse.Core.Model;

public sealed record CurrentWarehouseState(ItemId Id, ItemAvailability Availability)
{
    public AvailableQuantity GetAvailableQuantity()
    {
        return Availability.GetAvailableQuantity();
    }
}