using System.ComponentModel.DataAnnotations;

namespace BookingFlightServer.Validations
{
	public class CompareStringWithOtherAttribute : ValidationAttribute
	{
		private readonly string _otherProperty;
		public CompareStringWithOtherAttribute(string otherProperty)
		{
			_otherProperty = otherProperty;
		}
		protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
		{
			if (value == null) return ValidationResult.Success;
			var otherPropertyValue = validationContext.ObjectType.GetProperty(_otherProperty)?.GetValue(validationContext.ObjectInstance, null);
			if (value.ToString() == otherPropertyValue?.ToString())
			{
				return new ValidationResult(ErrorMessage ?? $"{validationContext.DisplayName} không thể trùng với {_otherProperty}");
			}
			return ValidationResult.Success;
		}
	}
}
