using DataTypeMapping.Dto;
using System.ComponentModel.DataAnnotations;
namespace AuthServer.Utilities.CustomValidators
{
    public class AddressRequiredIfAnyFilledAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var address = value as AddressDto;
            if (address == null) return ValidationResult.Success; // Address skipped entirely

            var allEmpty = string.IsNullOrWhiteSpace(address.StreetLine1)
                           && string.IsNullOrWhiteSpace(address.StreetLine2)
                           && string.IsNullOrWhiteSpace(address.City)
                           && string.IsNullOrWhiteSpace(address.State)
                           && string.IsNullOrWhiteSpace(address.PinCode);

            var allFilled = !string.IsNullOrWhiteSpace(address.StreetLine1)
                            && !string.IsNullOrWhiteSpace(address.City)
                            && !string.IsNullOrWhiteSpace(address.State)
                            && !string.IsNullOrWhiteSpace(address.PinCode);

            if (!allEmpty && !allFilled)
            {
                return new ValidationResult("Either provide a complete address or leave it empty.");
            }

            return ValidationResult.Success;
        }
    }
}
