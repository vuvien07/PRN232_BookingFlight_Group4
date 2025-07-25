using AutoMapper;
using BookingFlightServer.DTO.Shared;
using BookingFlightServer.Entities;

namespace BookingFlightServer.Mappers
{
	public class MappingProfile : Profile
	{
		public MappingProfile()
		{
			// Add custom mapping for DateOnly to DateTime
			CreateMap<DateOnly, DateTime>().ConvertUsing(src => src.ToDateTime(TimeOnly.MinValue));
			CreateMap<DateTime, DateOnly>().ConvertUsing(src => DateOnly.FromDateTime(src));

			CreateMap<Seat, SeatDTO>().ReverseMap();
			CreateMap<Flight, FlightDTO>()
				.ForMember(c => c.FromCode, opt => opt.MapFrom(c => c.DepartureAirport.AirportCode))
				.ForMember(c => c.ToCode, opt => opt.MapFrom(c => c.ArrivalAirport.AirportCode))
				.ForMember(c => c.FromName, opt => opt.MapFrom(c => c.DepartureAirport.AirportName))
				.ForMember(c => c.ToName, opt => opt.MapFrom(c => c.ArrivalAirport.AirportName))
				.ForMember(c => c.Manufacture, opt => opt.MapFrom(c => c.Plane.Manufacture))
				.ForMember(c => c.PlaneCode, opt => opt.MapFrom(c => c.Plane.PlaneCode))
				.ForMember(c => c.Model, opt => opt.MapFrom(c => c.Plane.Model))
				.ForMember(c => c.DepartureTime, opt => opt.MapFrom(c => c.DepartureTime.ToString()))
				.ForMember(c => c.ArrivalTime, opt => opt.MapFrom(c => c.ArrivalTime.ToString()))
				.ReverseMap();

			// New detailed DTOs for ticket display
			CreateMap<Airport, AirportDTO>().ReverseMap();
			CreateMap<Plane, PlaneDTO>()
				.ForMember(dest => dest.PlaneModel, opt => opt.MapFrom(src => src.Model))
				.ReverseMap();
			CreateMap<Flight, DetailedFlightDTO>()
				.ForMember(dest => dest.DepartureAirport, opt => opt.MapFrom(src => src.DepartureAirport))
				.ForMember(dest => dest.ArrivalAirport, opt => opt.MapFrom(src => src.ArrivalAirport))
				.ForMember(dest => dest.Plane, opt => opt.MapFrom(src => src.Plane))
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
				ReverseMap();


			CreateMap<Account, AccountDTO>()
				.ForMember(dest => dest.ManagerId, opt => opt.MapFrom(src => 
					src.RoleId == 4 && src.Manager != null ? src.Manager.ManagerId : (int?)null))
				.ReverseMap();

			CreateMap<Role, RoleDTO>().ReverseMap();
			CreateMap<Status, StatusDTO>().ReverseMap();
			

		}
	}
}
