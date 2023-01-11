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
        public IActionResult Add()
        {
            var model = new LocationShortInfoViewModel();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Add(LocationShortInfoViewModel model)
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

            await _locationService.AddAsync(dto);

            TempData["message"] = LocationAdded;
            return RedirectToAction(nameof(Add));
        }

        public async Task<IActionResult> ViewAll()
        {
            var dtos = await _locationService.GetAllAsync();

            IEnumerable<LocationWithCodeViewModel> models = dtos.Select(d => new LocationWithCodeViewModel
            {
                Id = d.Id,
                Name = d.Name,
                Code = d.Code
            })
                .ToList();
            return View(models);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int locationId)
        {
            var dto = await _locationService.GetByIdAsync(locationId);

            var model = new LocationWithCodeViewModel()
            {
                Id = dto.Id,
                Name = dto.Name,
                Code = dto.Code
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(LocationWithCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var dto = new LocationWithCodeDTO()
            {
                Id = model.Id,
                Name = model.Name,
                Code = model.Code
            };

            await _locationService.EditAsync(dto);

            return RedirectToAction(nameof(ViewAll));
        }

        public async Task<IActionResult> Delete(int locationId)
        {
            await _locationService.DeleteAsync(locationId);

            return RedirectToAction(nameof(ViewAll));
        }
    }
}
