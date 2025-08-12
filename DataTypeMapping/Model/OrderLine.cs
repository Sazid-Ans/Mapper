namespace DataTypeMapping.Model
{
    public class OrderLine
    {
        public string SKU { get; set; }
        public int Quantity { get; set; }
        public Money UnitPrice { get; set; }
        public Product Product { get; set; }
    }
}
