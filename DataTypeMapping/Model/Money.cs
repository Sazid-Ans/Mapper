namespace DataTypeMapping.Model
{
    public class Money
    {
        public Money(decimal discountPrice, string currency) 
        {
            Amount = discountPrice;
            Currency = currency;
        }
        public Money()
        {
            
        }
        public decimal Amount { get; set; }
        public string Currency { get; set; } // Assume "USD" unless specified
    }
}
