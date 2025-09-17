using DataTypeMapping.Model.Enum;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataTypeMapping.Model
{
    public class Order
    {
        public Guid Id { get; set; }

        // Foreign key
        // Foreign key only (no navigation to Customer here)
        public string CustomerId { get; set; }  // must match IdentityUser's key type (string)

        public List<OrderLine> Items { get; set; }
        public Shipment Shipment { get; set; }
        public DateTime CreatedDate { get; set; }
        public Money Total { get; set; }
        public OrderSataus Status { get; set; }
        public Payment Payment { get; set; }
    }
}
