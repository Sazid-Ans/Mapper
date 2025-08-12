namespace DataTypeMapping.Dto
{
    public class ProductDto
    {
        public string ProductId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string DiscountPercent { get; set; } // e.g., "10%"

    }
}
