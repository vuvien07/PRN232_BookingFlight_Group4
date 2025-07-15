using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Library
{
	public static class JwtDecoder
	{
		public static string GetRoleInToken(string token)
		{
			var handler = new JwtSecurityTokenHandler();
			var jwt = handler.ReadJwtToken(token);
			return jwt.Claims.First(c => c.Type == "Role").Value;
		}

		public static string GetUsernameFromToken(string token)
		{
			var handler = new JwtSecurityTokenHandler();
			var jwt = handler.ReadJwtToken(token);
			return jwt.Claims.First(c => c.Type == ClaimTypes.Name).Value;
		}
	}
}
