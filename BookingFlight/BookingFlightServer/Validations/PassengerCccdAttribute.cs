using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;

namespace BookingFlightServer.Validations
{
	public class PassengerCccdAttribute : ValidationAttribute
	{
		private readonly string _id;
		private readonly string _type;

		public PassengerCccdAttribute(string id, string type)
		{
			_id = id;
			_type = type;
		}
		protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
		{
			var container = validationContext.ObjectInstance;
			var idProp = container.GetType().GetProperty(_id);
			var typeProp = container.GetType().GetProperty(_type);
			var idValue = idProp?.GetValue(container);
			var typeValue = typeProp?.GetValue(container);
			if (value is not string cccd || idValue is not int id || typeValue is not string type)
			{
				return new ValidationResult("Dữ liệu không hợp lệ.");
			}
			if (cccd.IsNullOrEmpty())
			{
				switch (type)
				{
					case "Adult":
						return new ValidationResult("Vui lòng nhập căn cccd người lớn", new[] { $"Adult{id}" });
					default:
						return ValidationResult.Success;
				}
			}
			return ValidationResult.Success;
		}
	}
}
