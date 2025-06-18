using System.IdentityModel.Tokens.Jwt;

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
	}
}
