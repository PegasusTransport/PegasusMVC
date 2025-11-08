using Pegasus_MVC.DTO;
using Pegasus_MVC.Response;
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
                //var booking = CreateBookingDto(newBooking);

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

                    var previewResponse = apiResponse.Data;

            
                    logger.LogInformation("API Response: Distance={Distance}, Duration={Duration}, Price={Price}",
                        previewResponse.DistanceKm, previewResponse.DurationMinutes, previewResponse.Price);

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
                        DistanceKm = previewResponse.DistanceKm,
                        DurationMinutes = previewResponse.DurationMinutes,
                        Price = previewResponse.Price
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
        private static BookingPreviewRequestDto CreatePreview(CreateBookingVM createBooking)
        {
  
            double? ParseDoubleOrNull(string? value)
            {
                if (string.IsNullOrWhiteSpace(value))
                    return null;

                if (double.TryParse(value, CultureInfo.InvariantCulture, out double result))
                    return result;

                return null;
            }

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
            logger.LogInformation("=== Creating BookingDto ===");
            logger.LogInformation("Customer Info - FirstName: {FirstName}, LastName: {LastName}, Email: {Email}, Phone: {Phone}",
                createBooking.FirstName, createBooking.LastName, createBooking.Email, createBooking.PhoneNumber);

            logger.LogInformation("PickUp - DateTime: {DateTime}, Address: {Address}",
                createBooking.PickUpDateTime, createBooking.PickUpAddress);
            logger.LogInformation("PickUp Coordinates - Lat: {Lat}, Long: {Long}",
                createBooking.PickUpLatitude, createBooking.PickUpLongitude);

            logger.LogInformation("DropOff - Address: {Address}",
                createBooking.DropOffAddress);
            logger.LogInformation("DropOff Coordinates - Lat: {Lat}, Long: {Long}",
                createBooking.DropOffLatitude, createBooking.DropOffLongitude);

            logger.LogInformation("FirstStop - Address: {Address}, Lat: {Lat}, Long: {Long}",
                createBooking.FirstStop ?? "NULL",
                createBooking.FirstStopLatitude ?? "NULL",
                createBooking.FirstStopLongitude ?? "NULL");

            logger.LogInformation("SecondStop - Address: {Address}, Lat: {Lat}, Long: {Long}",
                createBooking.SecStop ?? "NULL",
                createBooking.SecStopLatitude ?? "NULL",
                createBooking.SecStopLongitude ?? "NULL");

            logger.LogInformation("Optional - Flightnumber: {Flight}, Comment: {Comment}",
                createBooking.Flightnumber ?? "NULL", createBooking.Comment ?? "NULL");

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

                FirstStopAddress = !string.IsNullOrEmpty(createBooking.FirstStop) ? createBooking.FirstStop : null,
                FirstStopLatitude = !string.IsNullOrEmpty(createBooking.FirstStopLatitude)
                    ? double.Parse(createBooking.FirstStopLatitude, CultureInfo.InvariantCulture)
                    : null,
                FirstStopLongitude = !string.IsNullOrEmpty(createBooking.FirstStopLongitude)
                    ? double.Parse(createBooking.FirstStopLongitude, CultureInfo.InvariantCulture)
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

            logger.LogInformation("=== BookingDto Created Successfully ===");
            logger.LogInformation("Parsed Coordinates - PickUp Lat: {PLat}, Long: {PLong} | DropOff Lat: {DLat}, Long: {DLong}",
                newBooking.PickUpLatitude, newBooking.PickUpLongitude,
                newBooking.DropOffLatitude, newBooking.DropOffLongitude);

            return newBooking;
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
