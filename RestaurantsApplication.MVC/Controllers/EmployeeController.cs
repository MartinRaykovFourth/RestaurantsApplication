using Microsoft.AspNetCore.Mvc;
using RestaurantsApplication.DTOs.DepartmentDTOs;
using RestaurantsApplication.DTOs.EmployeeDTOs;
using RestaurantsApplication.DTOs.EmploymentDTOs;
using RestaurantsApplication.MVC.Models.Department;
using RestaurantsApplication.MVC.Models.Employee;
using RestaurantsApplication.Services.Contracts;
using RestaurantsApplication.Services.Services;
using static RestaurantsApplication.MVC.Messages.SuccessMessages;

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
        public IActionResult AddEmployee()
        {
            var model = new EmployeeShortInfoViewModel();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddEmployee(EmployeeShortInfoViewModel model)
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

            await _employeeService.AddEmployeeAsync(dto);

            TempData["message"] = EmployeeAdded;
            return RedirectToAction(nameof(AddEmployee));
        }

        public async Task<IActionResult> ViewAll()
        {
            var dtos = await _employeeService.GetAllAsync();

            IEnumerable<EmployeeWithIdViewModel> models = dtos.Select(d => new EmployeeWithIdViewModel
            {
                Id = d.Id,
                FirstName = d.FirstName,
                LastName = d.LastName,
                Code = d.Code
            })
                .ToList();

            return View(models);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int employeeId)
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

        [HttpPost]
        public async Task<IActionResult> Edit(EmployeeWithIdViewModel model)
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
    }
}
