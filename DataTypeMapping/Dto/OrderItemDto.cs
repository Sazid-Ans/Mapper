namespace DataTypeMapping.Dto
{
    public class OrderItemDto
    {
        public string Sku { get; set; }
        public string Quantity { get; set; } // Stored as string in API
        public decimal UnitPrice { get; set; }
        public ProductDto ProductDetails { get; set; }

    }
}
