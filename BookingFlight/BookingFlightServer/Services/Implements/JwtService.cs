using BookingFlightServer.DTO;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BookingFlightServer.Services.Implements
{
	public class JwtService : IJwtService
	{
		private readonly IConfiguration _configuration;

		public JwtService(IConfiguration configuration)
		{
			_configuration = configuration;
		}
		public string CreateJwtToken(AccountDTO accountDTO)
		{
			DateTime expiration = DateTime.Now.AddMinutes(Convert.ToDouble(_configuration["Jwt:Expiration_In_Minutes"]));
			Claim[] claims = new Claim[]
			{
				new Claim(ClaimTypes.Name, accountDTO.Username.ToString()),
				new Claim(ClaimTypes.Role, accountDTO?.Role?.RoleName.ToString() ?? string.Empty)
			};
			SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? string.Empty));
			SigningCredentials credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
			JwtSecurityToken token = new JwtSecurityToken(
				_configuration["Jwt:Issuer"],
				_configuration["Jwt:Audience"],
				claims,
				expires: expiration,
				signingCredentials: credentials
				);
			JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
			string tokenString = handler.WriteToken(token);
			return tokenString;
		}

		public string CreateRefreshToken()
		{
			return Guid.NewGuid().ToString();
		}

		public double GetTokenExpirationTime()
		{
			return Convert.ToDouble(_configuration["Jwt:RefreshTokenExpiration_In_Minutes"]);
		}
	}
}
