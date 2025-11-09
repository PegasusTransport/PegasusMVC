using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;
using Pegasus_MVC.Services.Interfaces;
using Pegasus_MVC.Services.ValidationsErrors;
using Pegasus_MVC.ViewModels;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using System.Threading.Tasks;

namespace Pegasus_MVC.Services
{
    public class BookingValidationService(IHttpClientFactory httpClient, IValidator<CreateBookingVM> validator, ILogger<BookingValidationService> logger) : IBookingValidationService
    {
        private readonly HttpClient _httpClient = httpClient.CreateClient("PegasusServer");
        public async Task ValidateBookingAsync(CreateBookingVM booking, ModelStateDictionary modelState)
        {
            var arlandaCheck = ValidateArlandaRequirement(booking);

            if (arlandaCheck != null)
                modelState.AddModelError(arlandaCheck.ErrorName, arlandaCheck.ErrorMessage);
            var results = await validator.ValidateAsync(booking);



            if (!string.IsNullOrEmpty(booking.FirstStop))
            {
                logger.LogWarning($"Validating first stop: {booking.FirstStop}");

                if (string.IsNullOrEmpty(booking.FirstStopPlaceId))
                {
                    modelState.AddModelError("FirstStop", "The provided extra stops are invalid.");
                }
                else
                {
                    var isValid = await BeValidCoordinate(booking.FirstStopPlaceId);
                    if (isValid != null)
                        modelState.AddModelError("FirstStop", isValid.ErrorMessage);
                }
            }
            if (!string.IsNullOrEmpty(booking.SecStop))
            {
                logger.LogWarning($"Validating first stop: {booking.SecStop}");

                if (string.IsNullOrEmpty(booking.SecStopPlaceId))
                {
                    modelState.AddModelError("SecStop", "The provided extra stops are invalid.");
                }
                else
                {
                    var isValid = await BeValidCoordinate(booking.SecStopPlaceId);
                    if (isValid != null)
                        modelState.AddModelError("SecStop", isValid.ErrorMessage);
                }
            }


            var extraStops = new[]
            {
                new { Address = booking.FirstStop, PlaceId = booking.FirstStopPlaceId, FieldName = "FirstStop", LogName = "first stop" },
                new { Address = booking.SecStop, PlaceId = booking.SecStopPlaceId, FieldName = "SecStop", LogName = "second stop" }
            };

            foreach (var stop in extraStops)
            {
                await ValidateExtraStop(stop.Address, stop.PlaceId, stop.FieldName, stop.LogName, modelState);
            }

            foreach (var error in results.Errors)
            {
                modelState.AddModelError(error.PropertyName, error.ErrorMessage);
            }
          

        }
        private ErrorValidation? ValidateArlandaRequirement(CreateBookingVM bookingDto)
        {
            var allAddresses = GetAllAddresses(bookingDto);
            var hasArlanda = allAddresses.Any(a => a != null && a.Contains("arlanda", StringComparison.OrdinalIgnoreCase));

            if (!hasArlanda)
            {
                logger.LogWarning("Arlanda validation failed - no Arlanda address found");
                return new ErrorValidation() { ErrorName = "ValidateArlandaRequirement", ErrorMessage = "Arlanda validation failed - no Arlanda address found" };
            }

            if (!string.IsNullOrEmpty(bookingDto.PickUpAddress) &&
                bookingDto.PickUpAddress.Contains("arlanda", StringComparison.OrdinalIgnoreCase) &&
                string.IsNullOrEmpty(bookingDto.Flightnumber))
            {

                return new ErrorValidation() { ErrorName = "FlightNumberRequierment", ErrorMessage = "If pickup address is Arlanda, flight number is requierd" };
            }

            return null;
        }
        private async Task ValidateExtraStop(string? stopAddress, string? stopPlaceId, string fieldName, string logName, ModelStateDictionary modelState)
        {
            if (string.IsNullOrEmpty(stopAddress)) return;

            logger.LogWarning($"Validating {logName}: {stopAddress}");

            if (string.IsNullOrEmpty(stopPlaceId))
            {
                modelState.AddModelError(fieldName, "The provided extra stops are invalid.");
                return;
            }

            var validationResult = await BeValidCoordinate(stopPlaceId);
            if (validationResult != null)
                modelState.AddModelError(fieldName, validationResult.ErrorMessage);
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
        private async Task<ErrorValidation?> BeValidCoordinate(string placeId)
        {
            if (string.IsNullOrEmpty(placeId))
                return new ErrorValidation() { ErrorName = "Extra stop is invalid", ErrorMessage = "The provided extra stops are invalid." };
            var encodedPlaceId = Uri.EscapeDataString(placeId);
            using var response = await _httpClient.GetAsync($"/api/Map/GetLongNLat?placeId={encodedPlaceId}");

            if (response.IsSuccessStatusCode)
            {
                return null;
            }
            return new ErrorValidation() { ErrorName = "Extra stop is invalid", ErrorMessage = "The provided extra stops are invalid." };
        }
    }
}
