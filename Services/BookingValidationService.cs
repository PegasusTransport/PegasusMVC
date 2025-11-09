using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using Pegasus_MVC.DTO.Wrapper;
using Pegasus_MVC.Services.Interfaces;
using Pegasus_MVC.ViewModels;
using System.ComponentModel.DataAnnotations;

namespace Pegasus_MVC.Services
{
    public class BookingValidationService(IValidator<CreateBookingVM> validator, ILogger<BookingValidationService> logger) : IBookingValidationService
    {
        public async Task ValidateBookingAsync(CreateBookingVM booking, ModelStateDictionary modelState)
        {
            var arlandaCheck = ValidateArlandaRequirement(booking);

            if (arlandaCheck != null)
                modelState.AddModelError(arlandaCheck.ErrorName, arlandaCheck.ErrorMessage);
            var results = await validator.ValidateAsync(booking);
            
            foreach (var error in results.Errors)
            {
                modelState.AddModelError(error.PropertyName, error.ErrorMessage);
            }
          

        }
        private ArlandaValidationError? ValidateArlandaRequirement(CreateBookingVM bookingDto)
        {
            var allAddresses = GetAllAddresses(bookingDto);
            var hasArlanda = allAddresses.Any(a => a != null && a.Contains("arlanda", StringComparison.OrdinalIgnoreCase));

            if (!hasArlanda)
            {
                logger.LogWarning("Arlanda validation failed - no Arlanda address found");
                return new ArlandaValidationError() { ErrorName = "ValidateArlandaRequirement", ErrorMessage = "Arlanda validation failed - no Arlanda address found" };
            }

            if (!string.IsNullOrEmpty(bookingDto.PickUpAddress) &&
                bookingDto.PickUpAddress.Contains("arlanda", StringComparison.OrdinalIgnoreCase) &&
                string.IsNullOrEmpty(bookingDto.Flightnumber))
            {

                return new ArlandaValidationError() { ErrorName = "FlightNumberRequierment", ErrorMessage = "If pickup address is Arlanda, flight number is requierd" };
            }

            return null;
        }
        private static List<string> GetAllAddresses(CreateBookingVM bookingDto)
        {
            var addresses = new List<string> { bookingDto.PickUpAddress };

            if (!string.IsNullOrEmpty(bookingDto.FirstStop))
                addresses.Add(bookingDto.FirstStop);

            if (!string.IsNullOrEmpty(bookingDto.SecStop))
                addresses.Add(bookingDto.SecStop);

            addresses.Add(bookingDto.DropOffAddress);

            return addresses;
        }
    }
}
