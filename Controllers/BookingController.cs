using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Pegasus_MVC.DTO;
using Pegasus_MVC.Services;
using Pegasus_MVC.ViewModels;
using System.Globalization;
using System.Net;
using System.Threading.Tasks;

namespace Pegasus_MVC.Controllers
{
    public class BookingController(IBookingService bookingService, IHttpClientFactory httpClient, IValidator<CreateBookingVM> validator, ILogger<BookingController> logger) : Controller
    {
        private readonly HttpClient _httpClient = httpClient.CreateClient("PegasusServer");

        public IActionResult Index()
        {
            return View(new CreateBookingVM());
        }
        public IActionResult ConfirmAndSendBooking()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Preview(CreateBookingVM createBooking)
        {
            try
            {
                logger.LogInformation("Preview called with PickUp: {PickUp}, DropOff: {DropOff}",
                    createBooking.PickUpAddress, createBooking.DropOffAddress);

                var previewResponse = await bookingService.GetPreview(createBooking);

                if (previewResponse.StatusCode != HttpStatusCode.OK || previewResponse.Data == null)
                {
                    logger.LogWarning("Failed to get preview data. Status: {Status}", previewResponse.StatusCode);
                    return Content("<div class='alert alert-danger'>Could not load preview. Please try again.</div>");
                }

                // VIKTIGT - LOGGA DATA SOM KOMMER TILLBAKA
                logger.LogInformation("Preview data: Distance={Distance}, Duration={Duration}, Price={Price}",
                    previewResponse.Data.DistanceKm,
                    previewResponse.Data.DurationMinutes,
                    previewResponse.Data.Price);

                return PartialView("_BookingPreview", previewResponse.Data);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in Preview action");
                return Content($"<div class='alert alert-danger'>Error: {ex.Message}</div>");
            }
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateBookingVM createBooking)
        {
            var results = await validator.ValidateAsync(createBooking);

  
            foreach (var error in results.Errors)
            {
                logger.LogError(error.ErrorMessage);
                ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
            }

            if (!bookingService.CheckArlandaRequirement(createBooking))
            {
                ModelState.AddModelError("ArlandaRequirement", "One address needs to be Arlanda, if pickup address is Arlanda flight number is requierd");
            }


            if (!ModelState.IsValid)
            {
                return View("Index", createBooking);
            }
            
            var booking = await bookingService.CreateBookingAsync(createBooking);


            logger.LogInformation($"Booking created with status code: {booking.StatusCode}");

            if (booking.StatusCode != HttpStatusCode.OK)
            {
                ViewBag.ErrorMessage = "Bookning didnt send";
                return View("Index", createBooking);
            }

            return View("ConfirmAndSendBooking", booking.Data);
        }
    }
}
