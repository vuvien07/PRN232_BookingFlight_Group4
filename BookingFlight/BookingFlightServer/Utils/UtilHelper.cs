using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace BookingFlightServer.Utils
{
	public class UtilHelper
	{
		public static Dictionary<string, string?> GetModelStateErrors(ModelStateDictionary modelState)
		{
			return modelState.ToDictionary(kvp => kvp.Key, kvp => kvp.Value?.Errors.FirstOrDefault()?.ErrorMessage);
		}
		public static Dictionary<string, string?> GetPassengerModelStateErrors(ModelStateDictionary modelState)
		{
			return modelState.ToDictionary(
	   kvp => ExtractFieldName(kvp.Key),
	   kvp => kvp.Value?.Errors.FirstOrDefault()?.ErrorMessage
   );
		}
		private static string ExtractFieldName(string key)
		{
			var parts = key.Split('.');

			if (parts.Length >= 3)
			{
				return $"{parts[1]}{parts[2]}";
			}

			return key;
		}

		public static string ParseFromDateTime(DateTime dt, string pattern)
		{
			return dt.ToString(pattern);
		}

		public static TimeOnly ParseToTime(string time)
		{
			return TimeOnly.Parse(time);
		}

		public static TimeOnly ParseFromDatetime(DateTime time)
		{
			return TimeOnly.FromDateTime(time);
		}

		public static string ParseFromDateOnly(DateOnly dateOnly)
		{
			return dateOnly.ToString("yyyy-MM-dd");
		}

		public static string GetFromSplitString(string target, string regex, int index)
		{
			string[] strings = target.Split(regex);
			return strings[index];
		}

		public static string generateRandomString(int length)
		{
			string characters = "0123456789";
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < length; i++)
			{
				int index = (new Random().Next(characters.Length));
				sb.Append(characters.ToCharArray()[index]);
			}
			return sb.ToString();
		}
		public static T DeseializeObject<T>(string json) where T : new()
		{
			var options = new JsonSerializerOptions
			{
				PropertyNameCaseInsensitive = true
			};
			return JsonSerializer.Deserialize<T>(json, options) ?? new T();
		}
		public static string SerializeObject<T>(T json)
		{
			var options = new JsonSerializerOptions
			{
				PropertyNameCaseInsensitive = true
			};
			return JsonSerializer.Serialize<T>(json);
		}
		public static string GetUnderlyingTypeName(Type type)
		{
			if (Nullable.GetUnderlyingType(type) != null)
			{
				return Nullable.GetUnderlyingType(type)!.Name; // e.g., "Int32", "Single"
			}
			return type.Name; // e.g., "String", "Int32", "Float"
		}
	}
}
