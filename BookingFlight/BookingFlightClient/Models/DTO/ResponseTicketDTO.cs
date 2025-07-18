using BookingFlightClient.Models.Shared;
using BookingFlightServer.DTO.Shared;

namespace BookingFlightClient.Models.DTO
{
	public class ResponseTicketDTO
	{
		public int TicketId { get; set; }

		public string TicketNumber { get; set; } = null!;

		public DateOnly BookingDate { get; set; }

		public decimal TotalPrice { get; set; }

		public string Gender { get; set; } = null!;

		public string Name { get; set; } = null!;

		public DateOnly DateOfBirth { get; set; }

		public string FullName { get; set; } = null!;

		public ClassSeatDTO ClassSeatDTO { get; set; } = null!;

		public string? ContactFullName { get; set; }

		public string? ContactPhone { get; set; }

		public string? ContactEmail { get; set; }

		public string? ContactAddress { get; set; }
		public FlightDTO FlightDTO { get; set; } = null!;
		public List<TicketItemDTO> TicketItems { get; set; } = new List<TicketItemDTO>();
	}
}
