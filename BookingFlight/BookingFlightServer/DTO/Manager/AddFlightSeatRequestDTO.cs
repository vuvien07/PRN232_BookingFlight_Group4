using System.ComponentModel.DataAnnotations;

namespace BookingFlightServer.DTO.Manager
{
    public class AddFlightSeatRequestDTO
    {
        [Required]
        public int SeatId { get; set; }

        public bool? IsSat { get; set; } = false;

        public int? TicketId { get; set; }
    }
}
