namespace DataTypeMapping.Model.Enum
{
    public enum OrderSataus
    {
        PendingPayment,   // Order placed but payment not received yet
        PaymentReceived,  // Payment done, waiting to process
        Processing,       // Order being prepared/packed
        Shipped,          // Handed over to carrier
        Completed,        // Delivered successfully
        Cancelled,        // Cancelled before shipping
        Refunded          // Payment refunded
    }
}
