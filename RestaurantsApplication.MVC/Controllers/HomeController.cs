using Microsoft.AspNetCore.Mvc;
using static RestaurantsApplication.MVC.Messages.ProcessErrorMessages;

namespace RestaurantsApplication.MVC.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Error(string message = DefaultError)
        {
            ViewBag.Message = message;
            return View();
        }
    }
}