using BookingFlightServer.DTO.Shared;

namespace BookingFlightServer.Services
{
	public interface IJwtService
	{
		string CreateJwtToken(AccountDTO accountDTO);
		string CreateRefreshToken();
		double GetTokenExpirationTime();
		string CreateJwtDTOToken<T>(T t);
	}
}
