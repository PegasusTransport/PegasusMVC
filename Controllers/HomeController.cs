using Microsoft.AspNetCore.Mvc;

namespace Pegasus_MVC.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        public IActionResult FAQ()
        {
            return View();
        }

        public IActionResult Error404()
        {
            Response.StatusCode = 404;
            return View();
        }
    }
}
