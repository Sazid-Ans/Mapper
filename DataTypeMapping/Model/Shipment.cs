using DataTypeMapping.Model.Enum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataTypeMapping.Model
{
    public class Shipment
    {
        [Key]
        public Guid ShipmentId { get; set; }

        [Required]
        public string TrackingNumber { get; set; }

        [Required]
        public string CarrierName { get; set; }

        public ShipmentStatus Status { get; set; }

        public DateTime EstimatedArrival { get; set; }

        // Foreign key to Order (1-1 relationship)
        public Guid OrderId { get; set; }

        [ForeignKey(nameof(OrderId))]
        public Order Order { get; set; }
    }
}
