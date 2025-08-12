namespace DataTypeMapping.Dto
{
    public class CustomerDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public AddressDto Address { get; set; }
        public ShipmentDto ShipmentDetails { get; set; }
        public OrderItemDto[] OrderItems { get; set; }
        public OrderStatus Status { get; set; }
    }
}
