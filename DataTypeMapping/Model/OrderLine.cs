namespace DataTypeMapping.Model
{
    public class OrderLine
    {
        public int orderLineID { get; set; }
        public string SKU { get; set; }
        public int Quantity { get; set; }
        public Money UnitPrice { get; set; }

        // Each line = one product
        public int ProductId { get; set; }
        public Product Product { get; set; }

        // Link back to Order
        public Guid OrderId { get; set; }
        public Order Order { get; set; }
    }
}
