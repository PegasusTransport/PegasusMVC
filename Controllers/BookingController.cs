using Microsoft.AspNetCore.Mvc;
using Pegasus_MVC.Models;

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
            if(createBooking.PickUpDateTime < DateTime.UtcNow.AddHours(48))
            {
                ModelState.AddModelError(nameof(createBooking.PickUpDateTime),
                    "Pickup date must be at least 48 hours from now.");
            }

            if (!ModelState.IsValid)
            {
                return View("Index", createBooking);
            }

            return RedirectToAction("ConfirmedBooking");
        }
    }
}
