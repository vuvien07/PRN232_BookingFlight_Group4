using AutoMapper;
using BookingFlightServer.DTO.Shared;
using BookingFlightServer.Entities;

namespace BookingFlightServer.Mappers
{
	public class MappingProfile : Profile
	{
		public MappingProfile()
		{
			CreateMap<Flight, FlightDTO>()
				.ForMember(c => c.FromCode, opt => opt.MapFrom(c => c.DepartureAirport.AirportCode))
				.ForMember(c => c.ToCode, opt => opt.MapFrom(c => c.ArrivalAirport.AirportCode))
				.ForMember(c => c.FromName, opt => opt.MapFrom(c => c.DepartureAirport.AirportName))
				.ForMember(c => c.ToName, opt => opt.MapFrom(c => c.ArrivalAirport.AirportName))
				.ForMember(c => c.Manufacture, opt => opt.MapFrom(c => c.Plane.Manufacture))
				.ForMember(c => c.PlaneCode, opt => opt.MapFrom(c => c.Plane.PlaneCode))
				.ForMember(c => c.Model, opt => opt.MapFrom(c => c.Plane.Model))
				.ReverseMap();
			CreateMap<ClassSeat, ClassSeatDTO>().ReverseMap();
			CreateMap<Customer, CustomerDTO>().ReverseMap();
			CreateMap<Item,  ItemDTO>().ReverseMap();
			CreateMap<TicketItem, TicketItemDTO>()
				.ForMember(c => c.Ticket, opt => opt.Ignore())
				.ForMember(c => c.Item, opt => opt.MapFrom(c => c.Item))
				.ReverseMap();
			CreateMap<Ticket, TicketDTO>().
				ForMember(c => c.ClassSeatDTO, opt => opt.MapFrom(c => c.ClassSeat)).
				ForMember(c => c.TicketItems, opt => opt.MapFrom(c => c.TicketItems)).
				ForMember(c => c.CustomerDTO, opt => opt.MapFrom(c => c.Customer)).
				ForMember(c => c.FlightDTO, opt => opt.MapFrom(c => c.Flight)).
				ForMember(c => c.ClassSeatDTO, opt => opt.MapFrom(c => c.ClassSeat)).
				ReverseMap();
			CreateMap<Role, RoleDTO>().ReverseMap();
			CreateMap<Status, StatusDTO>().ReverseMap();
			
		}
	}
}
