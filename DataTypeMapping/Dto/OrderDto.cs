using DataTypeMapping.Model;

namespace DataTypeMapping.Dto
{
    public class OrderDto
    {
        public Guid orderId { get; set; }  
        public int customerId { get; set; }
        public List<OrderItemDto> orderItems { get; set; }
        public ShipmentDto shipment { get; set; }
        public string orderDate { get; set; }
        public decimal totalAmount { get; set; }


    }
}
