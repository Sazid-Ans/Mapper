namespace DataTypeMapping.Model.Enum
{
    public enum ShipmentStatus
    {
        Pending = 0,       // Shipment not yet created
        Processing = 1,    // Preparing for dispatch
        Shipped = 2,       // Left the warehouse
        InTransit = 3,     // On the way to delivery address
        OutForDelivery = 4,// Carrier is delivering today
        Delivered = 5,     // Delivered successfully
        Returned = 6,      // Returned by the customer
    }
}
