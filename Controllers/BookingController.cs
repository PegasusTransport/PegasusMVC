using Microsoft.AspNetCore.Mvc;
using Pegasus_MVC.DTO;
using Pegasus_MVC.ViewModels;
using System.Globalization;

namespace Pegasus_MVC.Controllers
{
    public class BookingController : Controller
    {
        public IActionResult Index()
        {
            return View(new CreateBookingVM());
        }
        public IActionResult ConfirmedBooking()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(CreateBookingVM createBooking)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                // Lägg till alla fel som ett generellt meddelande så du ser dem
                foreach (var error in errors)
                {
                    ModelState.AddModelError("", error);
                }
            }
            if (createBooking.PickUpDateTime < DateTime.UtcNow.AddHours(48))
            {
                ModelState.AddModelError(nameof(createBooking.PickUpDateTime),
                    "Pickup date must be at least 48 hours from now.");
            }

            if (!ModelState.IsValid)
            {
                return View("Index", createBooking);
            }
            // REFACTOR LATER TO SERVICE LAYER
            // DOnt forgot IDo key in header

            

            return View("ConfirmedBooking", createBooking);
        }
    }
}
