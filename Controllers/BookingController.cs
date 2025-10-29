using Microsoft.AspNetCore.Mvc;

namespace Pegasus_MVC.Controllers
{
    public class BookingController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
