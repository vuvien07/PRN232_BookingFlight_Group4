using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace BookingFlightServer.Validations
{
	public class EmailContactAttribute : ValidationAttribute
	{
		private readonly string[] _ignorePaths;

		public EmailContactAttribute(params string[] ignorePaths)
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

			if (value is not string email)
			{
				return new ValidationResult("Dữ liệu không hợp lệ.");
			}

			if (string.IsNullOrEmpty(email))
			{
				return new ValidationResult("Email liên hệ không được để trống");
			}
			if (!Regex.IsMatch(email, @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$"))
			{
				return new ValidationResult("Email liên hệ không đúng định dạng");
			}
			return ValidationResult.Success;
		}
	}
}
