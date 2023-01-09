using Microsoft.AspNetCore.Mvc;
using RestaurantsApplication.DTOs.DepartmentDTOs;
using RestaurantsApplication.MVC.Models.Department;
using RestaurantsApplication.Services.Contracts;
using static RestaurantsApplication.MVC.Messages.SuccessMessages;

namespace RestaurantsApplication.MVC.Controllers
{
    public class DepartmentController : Controller
    {
        private readonly ILocationService _locationService;
        private readonly IDepartmentService _departmentService;

        public DepartmentController(ILocationService locationService, IDepartmentService departmentService)
        {
            _locationService = locationService;
            _departmentService = departmentService;
        }

        [HttpGet]
        public async Task<IActionResult> Add()
        {
            var model = new DepartmentShortInfoViewModel()
            {
                Locations = await _locationService.GetLocationsWithIdsAsync()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Add(DepartmentShortInfoViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var dto = new DepartmentShortInfoDTO()
            {
                Name = model.Name,
                LocationId = model.LocationId
            };

            await _departmentService.AddAsync(dto);

            TempData["message"] = DepartmentAdded;
            return RedirectToAction(nameof(Add));
        }

        public async Task<IActionResult> ViewAll()
        {
            var dtos = await _departmentService.GetAllAsync();

            IEnumerable<DepartmentWithLocationViewModel> models = dtos.Select(d => new DepartmentWithLocationViewModel
            {
                Id = d.Id,
                Name = d.Name,
                LocationName = d.LocationName
            })
                .ToList();

            return View(models);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int departmentId)
        {
            var dto = await _departmentService.GetByIdAsync(departmentId);

            var model = new DepartmentFullInfoViewModel()
            {
                Id = dto.Id,
                Name = dto.Name,
                LocationId=dto.LocationId,
                Locations = await _locationService.GetLocationsWithIdsAsync()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(DepartmentFullInfoViewModel model)
        {
            if(!ModelState.IsValid)
            {
                return View(model);
            }

            var dto = new DepartmentFullInfoDTO()
            {
                Id = model.Id,
                Name = model.Name,
                LocationId = model.LocationId
            };

            await _departmentService.EditAsync(dto);

            return RedirectToAction(nameof(ViewAll));
        }
    }
}
