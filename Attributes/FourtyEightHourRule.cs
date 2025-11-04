using System.ComponentModel.DataAnnotations;

namespace Pegasus_MVC.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class FourtyEightHourRule: ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is DateTime pickupTime)
                if (pickupTime < DateTime.UtcNow.AddHours(48))
                    return ValidationResult.Success;


            return new ValidationResult("Pickup date must be at least 48 hours from now.");
        }
    }
}
