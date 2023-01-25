using Microsoft.AspNetCore.Mvc;
using RestaurantsApplication.DTOs.DepartmentDTOs;
using RestaurantsApplication.MVC.Models.Department;
using RestaurantsApplication.Services.Contracts;
using static RestaurantsApplication.MVC.Messages.SuccessMessages;
using static RestaurantsApplication.MVC.Messages.ProcessErrorMessages;

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
            try
            {
                var model = new DepartmentShortInfoViewModel()
                {
                    Locations = await _locationService.GetLocationsWithIdsAsync()
                };

                return View(model);
            }
            catch (Exception)
            {
                return RedirectToAction("Error", "Home", new { message = CouldntLoadError });
            }
   
        }

        [HttpPost]
        public async Task<IActionResult> Add(DepartmentShortInfoViewModel model)
        {
            try
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
            catch (Exception)
            {
                return RedirectToAction("Error", "Home", new { message = CouldntProcessError });
            }

        }

        public async Task<IActionResult> ViewAll()
        {
            try
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
            catch (Exception)
            {
                return RedirectToAction("Error", "Home", new { message = RetrievingDepartmentsError });
            }

        }

        [HttpGet]
        public async Task<IActionResult> Edit(int departmentId)
        {
            try
            {
                var dto = await _departmentService.GetByIdAsync(departmentId);

                var model = new DepartmentFullInfoViewModel()
                {
                    Id = dto.Id,
                    Name = dto.Name,
                    LocationId = dto.LocationId,
                    Locations = await _locationService.GetLocationsWithIdsAsync()
                };

                return View(model);
            }
            catch (Exception)
            {
                return RedirectToAction("Error", "Home", new { message = CouldntLoadError });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(DepartmentFullInfoViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
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
            catch (Exception)
            {
                return RedirectToAction("Error", "Home", new { message = CouldntProcessError });
            }
        }

        public async Task<IActionResult> Delete(int departmentId)
        {
            try
            {
                await _departmentService.DeleteAsync(departmentId);

                return RedirectToAction(nameof(ViewAll));
            }
            catch (Exception)
            {
                return RedirectToAction("Error", "Home", new { message = CouldntProcessError });
            }
           
        }
    }
}
