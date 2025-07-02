using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace BookingFlightServer.Validations
{
	public class AddressContactAttribute : ValidationAttribute
	{
		private readonly string[] _ignorePaths;

		public AddressContactAttribute(params string[] ignorePaths)
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

			if (value is not string address)
			{
				return new ValidationResult("Dữ liệu không hợp lệ.");
			}

			if (string.IsNullOrEmpty(address))
			{
				return new ValidationResult("Địa chỉ liên hệ không được để trống");
			}
			return ValidationResult.Success;
		}
	}
}
