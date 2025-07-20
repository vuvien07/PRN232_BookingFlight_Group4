namespace BookingFlightClient.Models
{
    public class VnPayResponseModel
    {
        public bool Success { get; set; }
        public string PaymentStatus { get; set; } = string.Empty;
        public string OrderDescription { get; set; } = string.Empty;
        public string OrderId { get; set; } = string.Empty;
        public string PaymentId { get; set; } = string.Empty;
        public string TransactionId { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public string VnPayResponseCode { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime PaymentTime { get; set; }
    }
}
