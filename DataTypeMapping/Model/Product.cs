using DataTypeMapping.Model.Enum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataTypeMapping.Model
{
    public class Product
    {
        [Key]
        public Guid ProductId { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; }

        // Instead of persisting final discounted price, 
        // store base price + discount
        public Money BasePrice { get; set; }

        public decimal DiscountPercent { get; set; }

        [NotMapped] // Computed property
        public Money PriceAfterDiscount =>
            new Money(BasePrice.Amount - (BasePrice.Amount * DiscountPercent / 100), BasePrice.Currency);

        public Category Category { get; set; }

        // Navigation: One Product can appear in many OrderLines
        public List<OrderLine> OrderLines { get; set; } = new();
    }
}
