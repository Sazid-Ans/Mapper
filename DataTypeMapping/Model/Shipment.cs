using DataTypeMapping.Model.Enum;

namespace DataTypeMapping.Model
{
    public class Shipment
    {
        public string TrackingNumber { get; set; }
        public string CarrierName { get; set; }
        public ShipmentStatus Status { get; set; }
        public DateTime EstimatedArrival { get; set; }
    }
}
