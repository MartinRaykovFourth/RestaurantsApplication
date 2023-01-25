using Microsoft.AspNetCore.Mvc;
using RestaurantsApplication.DTOs.EmployeeDTOs;
using RestaurantsApplication.MVC.Models.Employee;
using RestaurantsApplication.Services.Contracts;
using static RestaurantsApplication.MVC.Messages.SuccessMessages;
using static RestaurantsApplication.MVC.Messages.ProcessErrorMessages;

namespace RestaurantsApplication.MVC.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly IEmployeeService _employeeService;

        public EmployeeController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        [HttpGet]
        public IActionResult Add()
        {
            var model = new EmployeeShortInfoViewModel();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Add(EmployeeShortInfoViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                var dto = new EmployeeShortInfoDTO()
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Code = model.Code
                };

                await _employeeService.AddAsync(dto);

                TempData["message"] = EmployeeAdded;
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
                var dtos = await _employeeService.GetAllAsync();

                IEnumerable<EmployeeWithIdViewModel> models = dtos
                    .Select(d => new EmployeeWithIdViewModel
                    {
                        Id = d.Id,
                        FirstName = d.FirstName,
                        LastName = d.LastName,
                        Code = d.Code
                    })
                    .ToList();

                return View(models);
            }
            catch (Exception)
            {
                return RedirectToAction("Error", "Home", new { message = RetrievingEmployeesError });
            }

        }

        [HttpGet]
        public async Task<IActionResult> Edit(int employeeId)
        {
            try
            {
                var dto = await _employeeService.GetByIdAsync(employeeId);

                var model = new EmployeeWithIdViewModel()
                {
                    Id = dto.Id,
                    FirstName = dto.FirstName,
                    LastName = dto.LastName,
                    Code = dto.Code
                };

                return View(model);
            }
            catch (Exception)
            {
                return RedirectToAction("Error", "Home", new { message = CouldntLoadError });
            }

        }

        [HttpPost]
        public async Task<IActionResult> Edit(EmployeeWithIdViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                var dto = new EmployeeWithIdDTO()
                {
                    Id = model.Id,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Code = model.Code
                };

                await _employeeService.EditAsync(dto);

                return RedirectToAction(nameof(ViewAll));
            }
            catch (Exception)
            {
                return RedirectToAction("Error", "Home", new { message = CouldntProcessError });
            }

        }

        public async Task<IActionResult> Delete(int employeeId)
        {
            try
            {
                await _employeeService.DeleteAsync(employeeId);

                return RedirectToAction(nameof(ViewAll));
            }
            catch (Exception)
            {
                return RedirectToAction("Error", "Home", new { message = CouldntProcessError });
            }

        }
    }
}
