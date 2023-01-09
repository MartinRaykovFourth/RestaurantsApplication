using Microsoft.AspNetCore.Mvc;

namespace RestaurantsApplication.MVC.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}