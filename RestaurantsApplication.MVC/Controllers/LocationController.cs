using Microsoft.AspNetCore.Mvc;
using RestaurantsApplication.DTOs.LocationDTOs;
using RestaurantsApplication.MVC.Models.Location;
using RestaurantsApplication.Services.Contracts;
using static RestaurantsApplication.MVC.Messages.SuccessMessages;

namespace RestaurantsApplication.MVC.Controllers
{
    public class LocationController : Controller
    {
        private readonly ILocationService _locationService;

        public LocationController(ILocationService locationService)
        {
            _locationService = locationService;
        }

        [HttpGet]
        public IActionResult AddLocation()
        {
            var model = new LocationShortInfoViewModel();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddLocation(LocationShortInfoViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var dto = new LocationShortInfoDTO
            {
                Name = model.Name,
                Code = model.Code
            };

            await _locationService.AddLocationAsync(dto);

            TempData["message"] = LocationAdded;
            return RedirectToAction(nameof(AddLocation));
        }
    }
}
