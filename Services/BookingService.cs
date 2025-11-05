using Pegasus_MVC.DTO;
using Pegasus_MVC.Response;
using Pegasus_MVC.ViewModels;
using System.Globalization;
using System.Net;

namespace Pegasus_MVC.Services
{
    public class BookingService(IHttpClientFactory httpClient, ILogger<BookingService> logger) : IBookingService
    {
        private readonly HttpClient _httpClient = httpClient.CreateClient("PegasusServer");
        public async Task<ServiceResponse<CreateBookingDto>> CreateBookingAsync(CreateBookingVM newBooking)
        {
            try
            {
                var booking = CreateBookingDto(newBooking);

                var request = new HttpRequestMessage(HttpMethod.Post, $"{_httpClient.BaseAddress}Booking/createBooking")
                {
                    Content = JsonContent.Create(booking)
                };

                request.Headers.Add("Idempotency-Key", Guid.NewGuid().ToString());
                var response = await _httpClient.SendAsync(request);
                logger.LogInformation($"Sent request to api {request.Content}");

                if (response.IsSuccessStatusCode)
                {
                    logger.LogInformation("Booking created successfully.");
                    return ServiceResponse<CreateBookingDto>.SuccessResponse(
                        HttpStatusCode.OK, 
                        booking);
                }

                logger.LogWarning($"Failed to create booking. Status code: {response.StatusCode}");
                return ServiceResponse<CreateBookingDto>.FailResponse(
                    response.StatusCode);
            }
            catch (Exception ex)
            {
                logger.LogError($"Error sending request to api: {ex.Message}");
                return ServiceResponse<CreateBookingDto>.FailResponse(HttpStatusCode.BadRequest);
            }

        }
        private static CreateBookingDto CreateBookingDto(CreateBookingVM createBooking)
        {
            var newBooking = new CreateBookingDto
            {
                FirstName = createBooking.FirstName,
                LastName = createBooking.LastName,
                Email = createBooking.Email,
                PhoneNumber = createBooking.PhoneNumber,

                PickUpDateTime = createBooking.PickUpDateTime,
                PickUpAddress = createBooking.PickUpAddress,


                PickUpLatitude = double.Parse(createBooking.PickUpLatitude, CultureInfo.InvariantCulture),
                PickUpLongitude = double.Parse(createBooking.PickUpLongitude, CultureInfo.InvariantCulture),

                DropOffAddress = createBooking.DropOffAddress,
                DropOffLatitude = double.Parse(createBooking.DropOffLatitude, CultureInfo.InvariantCulture),
                DropOffLongitude = double.Parse(createBooking.DropOffLongitude, CultureInfo.InvariantCulture),


                FirstStopAddress = !string.IsNullOrEmpty(createBooking.SecStop) ? createBooking.SecStop : null,
                FirstStopLatitude = !string.IsNullOrEmpty(createBooking.SecStopLatitude)
                    ? double.Parse(createBooking.SecStopLatitude, CultureInfo.InvariantCulture)
                    : null,
                FirstStopLongitude = !string.IsNullOrEmpty(createBooking.SecStopLongitude)
                    ? double.Parse(createBooking.SecStopLongitude, CultureInfo.InvariantCulture)
                    : null,


                SecondStopAddress = !string.IsNullOrEmpty(createBooking.SecStop) ? createBooking.SecStop : null,
                SecondStopLatitude = !string.IsNullOrEmpty(createBooking.SecStopLatitude)
                    ? double.Parse(createBooking.SecStopLatitude, CultureInfo.InvariantCulture)
                    : null,
                SecondStopLongitude = !string.IsNullOrEmpty(createBooking.SecStopLongitude)
                    ? double.Parse(createBooking.SecStopLongitude, CultureInfo.InvariantCulture)
                    : null,

                Flightnumber = !string.IsNullOrEmpty(createBooking.Flightnumber) ? createBooking.Flightnumber : null,
                Comment = !string.IsNullOrEmpty(createBooking.Comment) ? createBooking.Comment : null

            };

            return newBooking;
        }
        public bool CheckArlandaRequirement(CreateBookingVM bookingDto)
        {
            var allAddresses = GetAllAddresses(bookingDto);
            var hasArlanda = allAddresses.Any(a => a != null && a.Contains("arlanda", StringComparison.OrdinalIgnoreCase));

            if (!hasArlanda)
            {
                logger.LogWarning("Arlanda validation failed - no Arlanda address found");
                return false;
            }

            if (!string.IsNullOrEmpty(bookingDto.PickUpAddress) &&
                bookingDto.PickUpAddress.Contains("arlanda", StringComparison.OrdinalIgnoreCase) &&
                string.IsNullOrEmpty(bookingDto.Flightnumber))
            {
                logger.LogWarning("Arlanda validation failed - no flight number for pickup from Arlanda");
                return false;
            }

            logger.LogInformation("Arlanda validation passed");
            return true;
        }
        private List<string> GetAllAddresses(CreateBookingVM bookingDto)
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
