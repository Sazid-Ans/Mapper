using DataTypeMapping.Model.Enum;

namespace DataTypeMapping.Model
{
    public class Product
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Money PriceAfterDiscount { get; set; }
        public Category Category { get; set; }
    }
}
