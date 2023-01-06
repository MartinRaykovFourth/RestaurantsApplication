using Microsoft.AspNetCore.Mvc;
using RestaurantsApplication.DTOs.EmployeeDTOs;
using RestaurantsApplication.MVC.Models.Employee;
using RestaurantsApplication.Services.Contracts;
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
    }
}
