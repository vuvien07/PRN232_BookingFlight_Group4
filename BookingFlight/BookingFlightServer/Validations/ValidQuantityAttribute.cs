using BookingFlightServer.DTO.Shared;
using System.ComponentModel.DataAnnotations;

namespace BookingFlightServer.Validations
{
	public class ValidQuantityAttribute : ValidationAttribute
	{
		private readonly string _serviceId;

		public ValidQuantityAttribute(string serviceId)
		{
			_serviceId = serviceId;
		}

		protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
		{
			if (value is not List<ItemDTO> items)
			{
				return new ValidationResult("Dữ liệu không hợp lệ.");
			}
			if(items.Count == 0)
			{
				return ValidationResult.Success!;
			}

			var container = validationContext.ObjectInstance;
			var serviceIdProp = container.GetType().GetProperty(_serviceId);
			if (serviceIdProp == null)
			{
				return new ValidationResult($"Không tìm thấy thuộc tính '{_serviceId}'.");
			}

			var serviceIdValue = serviceIdProp.GetValue(container);
			if (serviceIdValue is not int serviceId)
			{
				return new ValidationResult("ServiceId không hợp lệ.");
			}

			int totalQuantity = items.Sum(i => i.Quantity);

			if (serviceId == 1 && totalQuantity > 8)
			{
				string memberName = $"Services[ServiceId={serviceId}].Items";
				return new ValidationResult("Dịch vụ này cho phép số lượng tối đa là 8 mặt hàng", new[] { memberName });
			}
			if (serviceId == 3 && totalQuantity > 4)
			{
				string memberName = $"Services[ServiceId={serviceId}].Items";
				return new ValidationResult("Dịch vụ này cho phép số lượng tối đa là 4 mặt hàng", new [] { memberName });
			}

			return ValidationResult.Success!;
		}
	}
}
