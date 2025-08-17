namespace DataTypeMapping.Dto
{
    public class Invoice
    {
        public Guid InvoiceId { get; set; }
        public Guid OrderId { get; set; }
        public string InvoiceDate { get; set; } // e.g., "2025-08-12"
        public decimal TotalAmount { get; set; }
        public decimal TaxAmount { get; set; }
        public PaymentDto PaymentDetails { get; set; }
    }
}
