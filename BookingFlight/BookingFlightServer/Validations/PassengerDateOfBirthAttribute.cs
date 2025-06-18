using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;

namespace BookingFlightServer.Validations
{
	public class PassengerDateOfBirthAttribute : ValidationAttribute
	{
		private readonly string _id;
		private readonly string _type;

		public PassengerDateOfBirthAttribute(string id, string type)
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
			if (value is not string dob || idValue is not int id || typeValue is not string type)
			{
				return new ValidationResult("Dữ liệu không hợp lệ.");
			}
			if (string.IsNullOrEmpty(dob))
			{
				switch (type)
				{
					case "Adult":
						return new ValidationResult("Vui lòng nhập ngày sinh người lớn", new[] { $"Adult{id}" });
					case "Child":
						return new ValidationResult("Vui lòng nhập ngày sinh trẻ em", new[] { $"Child{id}" });
					case "Infant":
						return new ValidationResult("Vui lòng nhập ngày sinh em bé", new[] { $"Baby{id}" });
				}
			}
			DateTime now = DateTime.Now;
			DateTime dateOfBirth = DateTime.Parse(dob);
			int age = now.Year - dateOfBirth.Year;
			switch (type)
			{
				case "Adult":
					if (age < 18)
					{
						return new ValidationResult("Năm sinh của người lớn phải lớn hơn hoặc bằng 18", new[] { $"Adult{id}" });
					}
					break;
				case "Child":
					if (age < 3 || age > 17)
					{
						return new ValidationResult("Năm sinh của trẻ em nằm trong khoảng 3 - 17", new[] { $"Child{id}" });
					}
					break;
				case "Infant":
					if (age > 2)
					{
						return new ValidationResult("Năm sinh của bé phải nhỏ hơn 3", new[] { $"Child{id}" });
					}
					break;

			}
			return ValidationResult.Success;
		}
	}
}
