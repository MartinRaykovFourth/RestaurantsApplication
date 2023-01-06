using Microsoft.AspNetCore.Mvc;
using RestaurantsApplication.MVC.Models;
using System.Diagnostics;

namespace RestaurantsApplication.MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}