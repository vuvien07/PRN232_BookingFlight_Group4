using AutoMapper;
using BookingFlightServer.DTO.Shared;
using BookingFlightServer.Entities;

namespace BookingFlightServer.Mappers
{
	public class MappingProfile : Profile
	{
		public MappingProfile()
		{
			CreateMap<Ticket, TicketDTO>().
				ForMember(c => c.CustomerDTO, opt => opt.MapFrom(c => c.Customer)).
				ForMember(c => c.FlightDTO, opt => opt.MapFrom(c => c.Flight)).
				ForMember(c => c.ClassSeatDTO, opt => opt.MapFrom(c => c.ClassSeat)).
				ReverseMap();
		}
	}
}
