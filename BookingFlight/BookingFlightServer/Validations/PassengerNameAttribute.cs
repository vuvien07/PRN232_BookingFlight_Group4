using Microsoft.IdentityModel.Tokens;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BookingFlightServer.Validations
{
	public class PassengerNameAttribute : ValidationAttribute
	{
		private readonly string _id;
		private readonly string _type;

		public PassengerNameAttribute(string id, string type)
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
			if(value is not string name || idValue is not int id || typeValue is not string type)
			{
				return new ValidationResult("Dữ liệu không hợp lệ.");
			}
			if (string.IsNullOrEmpty(name))
			{
				switch (type)
				{
					case "Adult":
						return new ValidationResult("Vui lòng nhập họ tên người lớn", new [] { $"Adult{id}" });
					case "Child":
						return new ValidationResult("Vui lòng nhập họ tên trẻ em", new[] { $"Child{id}" });
					case "Infant":
						return new ValidationResult("Vui lòng nhập họ tên em bé", new[] { $"Baby{id}" });
				}
			}
			return ValidationResult.Success;
		}
	}
}
