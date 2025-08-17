using DataTypeMapping.Model.Enum;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataTypeMapping.Model
{
    public class Order
    {
        public Guid Id { get; set; }

        // Foreign key
        public int CustomerId { get; set; }

        [ForeignKey(nameof(CustomerId))]
        public Customer Customer { get; set; }
        public List<OrderLine> Items { get; set; }
        public Shipment Shipment { get; set; }
        public DateTime CreatedDate { get; set; }
        public Money Total { get; set; }
        public OrderSataus Status { get; set; }
        public Payment Payment { get; set; }
    }
}
