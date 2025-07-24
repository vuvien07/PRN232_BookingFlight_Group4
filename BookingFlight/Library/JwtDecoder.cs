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

        public static int? GetAccountIdFromToken(string token)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jwt = handler.ReadJwtToken(token);
                var accountIdClaim = jwt.Claims.FirstOrDefault(c => c.Type == "AccountId");
                return accountIdClaim != null && int.TryParse(accountIdClaim.Value, out int accountId) ? accountId : null;
            }
            catch
            {
                return null;
            }
        }
    }
}
