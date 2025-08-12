using DataTypeMapping.Model.Enum;

namespace DataTypeMapping.Model
{
    public class Order
    {
        public Guid Id { get; set; }
        public Customer Customer { get; set; }
        public List<OrderLine> Items { get; set; }
        public Shipment Shipment { get; set; }
        public DateTime CreatedDate { get; set; }
        public OrderSataus Status { get; set; }
        public Money Total { get; set; }
    }
}
