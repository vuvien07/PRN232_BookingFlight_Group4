namespace BookingFlightServer.DTO
{
    public class UpdateSeatStatusRequest
    {
        public bool IsSat { get; set; }
        public int? TicketId { get; set; }
    }
}
