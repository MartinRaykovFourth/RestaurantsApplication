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
        public async Task<IActionResult> AddDepartment()
        {
            var model = new DepartmentShortInfoViewModel()
            {
                Locations = await _locationService.GetLocationsWithIdsAsync()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddDepartment(DepartmentShortInfoViewModel model)
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

            await _departmentService.AddDepartmentAsync(dto);

            TempData["message"] = DepartmentAdded;
            return RedirectToAction(nameof(AddDepartment));
        }
    }
}
