using System.ComponentModel.DataAnnotations.Schema;

namespace DataTypeMapping.Model
{
    public class Payment
    {
        public int PaymentId { get; set; }
        public Guid OrderId { get; set; }

        [ForeignKey(nameof(OrderId))]
        public Order Order { get; set; } // Navigation property to Order
        public Money Amount { get; set; } // Using Money class for amount
        public DateTime PaymentDate { get; set; } // Store as DateTime
        public string PaymentMethod { get; set; } // e.g., "CreditCard", "PayPal"
        public bool IsSuccessful { get; set; } // Indicates if the payment was successful

    }
}
