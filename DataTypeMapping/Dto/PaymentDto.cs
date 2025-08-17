namespace DataTypeMapping.Dto
{
    public class PaymentDto
    {
        public Guid PaymentId { get; set; }
        public int CustomerId { get; set; }
        public decimal Amount { get; set; }
        public string PaymentDate { get; set; } // API returns as string (e.g., "2025-08-12")
        public string PaymentMethod { get; set; } // e.g., "CreditCard", "PayPal"
        public bool IsSuccessful { get; set; }
    }
}
