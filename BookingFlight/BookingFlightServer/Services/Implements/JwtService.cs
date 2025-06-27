using BookingFlightServer.DTO.Shared;
using BookingFlightServer.Utils;
using Microsoft.IdentityModel.Tokens;
using System.Collections;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace BookingFlightServer.Services.Implements
{
	public class JwtService : IJwtService
	{
		private readonly IConfiguration _configuration;

		public JwtService(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		public string CreateJwtDTOToken<T>(T t)
		{
			DateTime expiration = DateTime.Now.AddMinutes(Convert.ToDouble(int.MaxValue));
			List<Claim> claims = new List<Claim>();
			claims.Add(new Claim("data", generatedJsonStringObject(t)));
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

		private string generatedJsonStringObject(object obj)
		{
			StringBuilder sb = new StringBuilder();
			sb.Append("{");
			var props = obj.GetType().GetProperties();
			for (int i = 0; i < props.Length; i++)
			{
				var prop = props[i];
				var name = prop.Name;
				var value = prop.GetValue(obj);
				sb.Append($"\"{name}\":");
				if (value == null)
				{
					var declareType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;

					if (declareType == typeof(string))
					{
						sb.Append("\"\"");
					}
					else if (typeof(System.Collections.IEnumerable).IsAssignableFrom(declareType) && declareType != typeof(string))
					{
						sb.Append("[]");
					}
					else if (declareType.IsValueType)
					{
						var defaultValue = Activator.CreateInstance(declareType);
						sb.Append(defaultValue?.ToString()?.ToLower() ?? "0");
					}
					else
					{
						sb.Append("{}");
					}
				}
				else
				{
					Type type = value.GetType();
					if (checkIsPrimitive(value))
					{
						if (value is string)
							sb.Append($"\"{value}\"");
						else { sb.Append($"{value}"); }
					}
					else if (value is System.Collections.IList list && !(value is string))
					{
						sb.Append("[");
						if (list.Count > 0)
						{
							List<string> items = new List<string>();
							foreach (var item in list)
							{
								if (item == null)
									items.Add("null");
								else if (checkIsPrimitive(item))
								{
									if (item is string || item is DateTime || item is char)
										items.Add($"\"{item}\"");
									else
										items.Add($"{item}");
								}
								else
									items.Add(generatedJsonStringObject(item)); // Gọi đệ quy
							}
							sb.Append(string.Join(",", items));
						}
						sb.Append("]");
					}
					else
					{
						sb.Append(generatedJsonStringObject(value));
					}
				}
				if (i < props.Length - 1)
				{
					sb.Append(",");
				}
			}
			sb.Append("}");
			return sb.ToString();
		}
		private bool checkIsPrimitive(object obj)
		{
			Type type = obj.GetType();
			return type.IsPrimitive || obj is string || obj is DateTime || obj is decimal || obj is bool;
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

		public Dictionary<string, object> DecodeJwtToken(string token)
		{
			var handler = new JwtSecurityTokenHandler();
			var jsonToken = handler.ReadToken(token);
			var tokenSecurity = jsonToken as JwtSecurityToken;

			if (tokenSecurity == null)
			{
				throw new ArgumentException("Invalid JWT token.");
			}

			return tokenSecurity.Claims.ToDictionary(claim => claim.Type, claim => (object)claim.Value);
		}
	}
}
