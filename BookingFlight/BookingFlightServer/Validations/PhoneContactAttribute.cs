using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace BookingFlightServer.Validations
{
	public class PhoneContactAttribute:ValidationAttribute
	{
		private readonly string[] _ignorePaths;

		public PhoneContactAttribute(params string[] ignorePaths)
		{
			_ignorePaths = ignorePaths;
		}

		protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
		{
			var httpContextAccessor = validationContext.GetService(typeof(IHttpContextAccessor)) as IHttpContextAccessor;

			var requestPath = httpContextAccessor?.HttpContext?.Request?.Path.Value ?? string.Empty;

			if (_ignorePaths.Any(p => requestPath.Contains(p)))
			{
				return ValidationResult.Success;
			}

			if (value is not string phone)
			{
				return new ValidationResult("Dữ liệu không hợp lệ.");
			}

			if (string.IsNullOrEmpty(phone))
			{
				return new ValidationResult("Số liên hệ không được để trống");
			}
			if (!Regex.IsMatch(phone, @"^(0|\+84)(3|5|7|8|9)+([0-9]{8})$"))
			{
				return new ValidationResult("Số liên hệ không đúng định dạng");
			}
			return ValidationResult.Success;
		}
	}
}
