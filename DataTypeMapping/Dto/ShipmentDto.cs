namespace DataTypeMapping.Dto
{
    public class ShipmentDto
    {
        public string TrackingNumber { get; set; }
        public string Carrier { get; set; }
        public string Status { get; set; } // e.g., "InTransit"
        public string EstimatedDelivery { get; set; } // e.g., "2025-08-15"

    }
}
