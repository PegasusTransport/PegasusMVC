using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Pegasus_MVC.DTO;
using Pegasus_MVC.Services.Interfaces;
using Pegasus_MVC.ViewModels;
using System.Globalization;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace Pegasus_MVC.Controllers
{
    public class BookingController(IBookingService bookingService, IBookingValidationService bookingValidation, IBookingStateService stateService) : Controller
    {
        
        public IActionResult Index()
        {
            return View(new CreateBookingVM());
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateBookingVM createBooking)
        {
            await bookingValidation.ValidateBookingAsync(createBooking, ModelState);
            if (!ModelState.IsValid)
                return View("Index", createBooking);

            var previewResponse = await bookingService.GetPreview(createBooking);
            if (previewResponse.StatusCode != HttpStatusCode.OK || previewResponse.Data == null)
            {
                ViewBag.ErrorMessage = "Could not calculate price. Please try again.";
                return View("Index", createBooking);
            }

            var bookingRequest = bookingService.CreateBookingDto(createBooking);
            var stateId = stateService.SaveBookingState(bookingRequest);
   
            ViewBag.StateId = stateId;
            return View("ConfirmBooking", previewResponse.Data);
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmBooking(string stateId)
        {
            var bookingRequest = stateService.GetBookingState(stateId);
            if (bookingRequest == null)
            {
                ViewBag.ErrorMessage = "Session expired. Please create booking again.";
                return RedirectToAction("Index");
            }

            var booking = await bookingService.CreateBookingAsync(bookingRequest);
            if (booking.StatusCode != HttpStatusCode.OK)
            {
                ViewBag.ErrorMessage = "Booking failed to send";
                return RedirectToAction("Index");
            }

            return View("BookingSuccess", booking.Data);
        }
    }
}
