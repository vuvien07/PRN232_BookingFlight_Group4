using System.ComponentModel.DataAnnotations;

namespace BookingFlightServer.Validations
{
    public class DifferentAirportsAttribute : ValidationAttribute
    {
        private readonly string _comparisonProperty;

        public DifferentAirportsAttribute(string comparisonProperty)
        {
            _comparisonProperty = comparisonProperty;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
                return ValidationResult.Success;

            var currentValue = (int)value;
            var property = validationContext.ObjectType.GetProperty(_comparisonProperty);

            if (property == null)
                throw new ArgumentException($"Property {_comparisonProperty} not found");

            var comparisonValue = (int?)property.GetValue(validationContext.ObjectInstance);

            if (comparisonValue.HasValue && currentValue == comparisonValue.Value)
            {
                return new ValidationResult(ErrorMessage ?? "Departure and Arrival airports must be different");
            }

            return ValidationResult.Success;
        }
    }
}
