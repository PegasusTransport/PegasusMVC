using Pegasus_MVC.DTO;
using Pegasus_MVC.DTO.Wrapper;
using Pegasus_MVC.Response;
using Pegasus_MVC.Services.Interfaces;
using Pegasus_MVC.ViewModels;
using System.Globalization;
using System.Net;
using System.Runtime.CompilerServices;

namespace Pegasus_MVC.Services
{
    public class BookingService(IHttpClientFactory httpClient, ILogger<BookingService> logger) : IBookingService
    {
        private readonly HttpClient _httpClient = httpClient.CreateClient("PegasusServer");
        public async Task<ServiceResponse<CreateBookingDto>> CreateBookingAsync(CreateBookingDto bookingRequest)
        {
            try
            {

                var request = new HttpRequestMessage(HttpMethod.Post, $"{_httpClient.BaseAddress}Booking/createBooking")
                {
                    Content = JsonContent.Create(bookingRequest)
                };

                request.Headers.Add("Idempotency-Key", Guid.NewGuid().ToString());
                var response = await _httpClient.SendAsync(request);
                logger.LogInformation($"Sent request to api {request.Content}");

                if (response.IsSuccessStatusCode)
                {
                    logger.LogInformation("Booking created successfully.");
                    return ServiceResponse<CreateBookingDto>.SuccessResponse(
                        HttpStatusCode.OK,
                        bookingRequest);
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
        public async Task<ServiceResponse<BookingPreviewVM>> GetPreview(CreateBookingVM newBooking)
        {
            try
            {
            
                if (string.IsNullOrWhiteSpace(newBooking.PickUpLatitude) ||
                    string.IsNullOrWhiteSpace(newBooking.PickUpLongitude) ||
                    string.IsNullOrWhiteSpace(newBooking.DropOffLatitude) ||
                    string.IsNullOrWhiteSpace(newBooking.DropOffLongitude))
                {
                    logger.LogWarning("Missing required coordinates for preview");
                    return ServiceResponse<BookingPreviewVM>.FailResponse(HttpStatusCode.BadRequest);
                }

                var preview = CreatePreview(newBooking);

                logger.LogInformation("Calling API with preview request");

                var request = new HttpRequestMessage(HttpMethod.Post, $"{_httpClient.BaseAddress}Booking/previewBookingPrice")
                {
                    Content = JsonContent.Create(preview)
                };

                var response = await _httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {

                    var apiResponse = await response.Content.ReadFromJsonAsync<ApiServiceResponse<BookingPreviewResponseDto>>();

                    if (apiResponse?.Data == null)
                    {
                        logger.LogWarning("Preview response was null");
                        return ServiceResponse<BookingPreviewVM>.FailResponse(HttpStatusCode.InternalServerError);
                    }

                    logger.LogInformation("API Response: Distance={Distance}, Duration={Duration}, Price={Price}",
                        apiResponse.Data.DistanceKm, apiResponse.Data.DurationMinutes, apiResponse.Data.Price);

                    var previewVM = new BookingPreviewVM 
                    {
                        Email = newBooking.Email,
                        FirstName = newBooking.FirstName,
                        LastName = newBooking.LastName,
                        PhoneNumber = newBooking.PhoneNumber,
                        PickUpDateTime = newBooking.PickUpDateTime,
                        PickUpAddress = newBooking.PickUpAddress,
                        FirstStopAddress = newBooking.FirstStop,
                        SecondStopAddress = newBooking.SecStop,
                        DropOffAddress = newBooking.DropOffAddress,
                        Flightnumber = newBooking.Flightnumber,
                        Comment = newBooking.Comment,
                        DistanceKm = apiResponse.Data.DistanceKm,
                        DurationMinutes = apiResponse.Data.DurationMinutes,
                        Price = apiResponse.Data.Price
                    };

                    logger.LogInformation("Created VM: Distance={Distance}, Duration={Duration}, Price={Price}",
                        previewVM.DistanceKm, previewVM.DurationMinutes, previewVM.Price);

                    return ServiceResponse<BookingPreviewVM>.SuccessResponse(
                        HttpStatusCode.OK,
                        previewVM);
                }

                logger.LogWarning($"Failed to get booking preview. Status code: {response.StatusCode}");
                return ServiceResponse<BookingPreviewVM>.FailResponse(response.StatusCode);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error sending request to api");
                return ServiceResponse<BookingPreviewVM>.FailResponse(HttpStatusCode.BadRequest);
            }
        }
        private static BookingPreviewRequestDto CreatePreview(CreateBookingVM createBooking)
        {
            var bookingPreviewRequest = new BookingPreviewRequestDto
            {
                PickUpDateTime = createBooking.PickUpDateTime,
                PickUpAddress = createBooking.PickUpAddress,
                PickUpLatitude = ParseDoubleOrNull(createBooking.PickUpLatitude) ?? 0,
                PickUpLongitude = ParseDoubleOrNull(createBooking.PickUpLongitude) ?? 0,

                DropOffAddress = createBooking.DropOffAddress,
                DropOffLatitude = ParseDoubleOrNull(createBooking.DropOffLatitude) ?? 0,
                DropOffLongitude = ParseDoubleOrNull(createBooking.DropOffLongitude) ?? 0,

                FirstStopAddress = !string.IsNullOrEmpty(createBooking.FirstStop) ? createBooking.FirstStop : null,
                FirstStopLatitude = ParseDoubleOrNull(createBooking.FirstStopLatitude),
                FirstStopLongitude = ParseDoubleOrNull(createBooking.FirstStopLongitude),

                SecondStopAddress = !string.IsNullOrEmpty(createBooking.SecStop) ? createBooking.SecStop : null,
                SecondStopLatitude = ParseDoubleOrNull(createBooking.SecStopLatitude),
                SecondStopLongitude = ParseDoubleOrNull(createBooking.SecStopLongitude),

                Flightnumber = !string.IsNullOrEmpty(createBooking.Flightnumber) ? createBooking.Flightnumber : null,
            };

            return bookingPreviewRequest;
        }
        public CreateBookingDto CreateBookingDto(CreateBookingVM createBooking)
        {  
            var newBooking = new CreateBookingDto
            {
                FirstName = createBooking.FirstName,
                LastName = createBooking.LastName,
                Email = createBooking.Email,
                PhoneNumber = createBooking.PhoneNumber,

                PickUpDateTime = createBooking.PickUpDateTime,
                PickUpAddress = createBooking.PickUpAddress,
                PickUpLatitude = ParseDoubleOrNull(createBooking.PickUpLatitude) ?? 0,
                PickUpLongitude = ParseDoubleOrNull(createBooking.PickUpLongitude) ?? 0,

                DropOffAddress = createBooking.DropOffAddress,
                DropOffLatitude = ParseDoubleOrNull(createBooking.DropOffLatitude) ?? 0,
                DropOffLongitude = ParseDoubleOrNull(createBooking.DropOffLongitude) ?? 0,

                FirstStopAddress = !string.IsNullOrEmpty(createBooking.FirstStop) ? createBooking.FirstStop : null,
                FirstStopLatitude = ParseDoubleOrNull(createBooking.FirstStopLatitude),
                FirstStopLongitude = ParseDoubleOrNull(createBooking.FirstStopLongitude),

                SecondStopAddress = !string.IsNullOrEmpty(createBooking.SecStop) ? createBooking.SecStop : null,
                SecondStopLatitude = ParseDoubleOrNull(createBooking.SecStopLatitude),
                SecondStopLongitude = ParseDoubleOrNull(createBooking.SecStopLongitude),

                Flightnumber = !string.IsNullOrEmpty(createBooking.Flightnumber) ? createBooking.Flightnumber : null,
                Comment = !string.IsNullOrEmpty(createBooking.Comment) ? createBooking.Comment : null
            };
            return newBooking;
        }

        private static double? ParseDoubleOrNull(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            if (double.TryParse(value, CultureInfo.InvariantCulture, out double result))
                return result;

            return null;
        }
    }
}
