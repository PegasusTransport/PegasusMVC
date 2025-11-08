using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Pegasus_MVC.DTO;
using Pegasus_MVC.Services;
using Pegasus_MVC.ViewModels;
using System.Globalization;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace Pegasus_MVC.Controllers
{
    public class BookingController(IBookingService bookingService, IValidator<CreateBookingVM> validator) : Controller
    {
        
        public IActionResult Index()
        {
            return View(new CreateBookingVM());
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateBookingVM createBooking)
        {
            var results = await validator.ValidateAsync(createBooking);

            foreach (var error in results.Errors)
            {
                ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
            }

            if (!bookingService.CheckArlandaRequirement(createBooking))
            {
                ModelState.AddModelError("ArlandaRequirement", "One address needs to be Arlanda, if pickup address is Arlanda flight number is required");
            }

            if (!ModelState.IsValid)
            {
                return View("Index", createBooking);
            }

            var previewResponse = await bookingService.GetPreview(createBooking);

            if (previewResponse.StatusCode != HttpStatusCode.OK || previewResponse.Data == null)
            {
                ViewBag.ErrorMessage = "Could not calculate price. Please try again.";
                return View("Index", createBooking);
            }

            // Save booking data to tempdata for confirmation
            var bookingRequest = bookingService.CreateBookingDto(createBooking);
            TempData["BookingRequest"] = JsonSerializer.Serialize(bookingRequest);

            return View("ConfirmBooking", previewResponse.Data);
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmBooking()
        {
            // Get booking from tempdata
            var bookingRequestJson = TempData["BookingRequest"] as string;

            if (string.IsNullOrEmpty(bookingRequestJson))
            {
                ViewBag.ErrorMessage = "Session expired. Please create booking again.";
                return RedirectToAction("Index");
            }

            var bookingRequest = JsonSerializer.Deserialize<CreateBookingDto>(bookingRequestJson);

            if (bookingRequest == null)
            {
                ViewBag.ErrorMessage = "Invalid booking data.";
                return RedirectToAction("Index");
            }

            var booking = await bookingService.CreateBookingAsync(bookingRequest);

            if (booking.StatusCode != HttpStatusCode.OK)
            {
                ViewBag.ErrorMessage = "Booking failed to send";

                TempData["BookingRequest"] = bookingRequestJson;
                return RedirectToAction("Index");
            }

            return View("BookingSuccess", booking.Data);
        }
    }
}
