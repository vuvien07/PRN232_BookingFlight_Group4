using BookingFlightServer.DTO;

namespace BookingFlightServer.Services.Implements
{
	public class CheckoutFlightService : ICheckoutFlightService
	{
		public decimal caculateServicePrice(List<ServiceDTO> serviceDTOs)
		{
			decimal total = 0;
			foreach (var service in serviceDTOs)
			{
				foreach (var item in service.Items)
				{
					total += item.Price;
				}
			}
			return total;
		}

		public int caculateTotalPerson(List<PreorderFlightDTO> preorderFlightDTOs)
		{
			return preorderFlightDTOs.Sum(p => p.Quantity);
		}
	}
}
