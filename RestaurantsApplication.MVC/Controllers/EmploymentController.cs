using Microsoft.AspNetCore.Mvc;
using RestaurantsApplication.DTOs.EmploymentDTOs;
using RestaurantsApplication.MVC.Models.Employment;
using RestaurantsApplication.Services.Contracts;
using static RestaurantsApplication.MVC.Messages.ErrorMessages;
using static RestaurantsApplication.MVC.Messages.SuccessMessages;

namespace RestaurantsApplication.MVC.Controllers
{
    public class EmploymentController : Controller
    {
        private readonly IRoleService _roleService;
        private readonly ILocationService _locationService;
        private readonly IDepartmentService _departmentService;
        private readonly IEmploymentService _employmentService;
        private readonly IValidatorService _validatorService;

        public EmploymentController(IRoleService roleService,
            ILocationService locationService,
            IDepartmentService departmentService,
            IValidatorService validatorService,
            IEmploymentService employmentService)
        {
            _roleService = roleService;
            _locationService = locationService;
            _departmentService = departmentService;
            _validatorService = validatorService;
            _employmentService = employmentService;
        }

        [HttpGet]
        public async Task<IActionResult> Add(string employeeCode)
        {
            var model = new EmploymentShortInfoViewModel()
            {
                StartDate = DateTime.Now,
                EmployeeCode = employeeCode,
                Roles = await _roleService.GetRolesWithIdsAsync(),
                Locations = await _locationService.GetLocationsWithIdsAsync(),
                Departments = await _departmentService.GetDepartmentsWithIdsAsync()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Add(EmploymentShortInfoViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await MapCollections(model);
                return View(model);
            }

            if (model.EndDate != null && model.EndDate < model.StartDate)
            {
                ModelState.AddModelError(nameof(model.EndDate), EarlierEndDateMessage);
            }

            if (!await _validatorService.ValidateDepartmentAsync(model.DepartmentId, model.LocationId))
            {
                ModelState.AddModelError(nameof(model.DepartmentId), DepartmentDoesntExistInLocationError);
            }

            model.EmployeeId = await _validatorService.ValidateEmployeeAsync(model.EmployeeCode);

            if (model.EmployeeId == 0)
            {
                ModelState.AddModelError(nameof(model.EmployeeCode), EmployeeWrongCodeError);
                await MapCollections(model);
                return View(model);
            }

            if (model.IsMain && !await _validatorService.CanEmploymentBeMainAsync(model.EmployeeId))
            {
                ModelState.AddModelError(nameof(model.IsMain), EmployeeAlreadyHasMainEmploymentError);
                await MapCollections(model);
                return View(model);
            }

            var dto = MapDTO(model);

            await _employmentService.AddAsync(dto);

            TempData["message"] = EmploymentAdded;
            return RedirectToAction("ViewAll","Employee");
        }

        private EmploymentShortInfoDTO MapDTO(EmploymentShortInfoViewModel model)
        {
            return new EmploymentShortInfoDTO()
            {
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                DepartmentId = model.DepartmentId,
                EmployeeId = model.EmployeeId,
                LocationId = model.LocationId,
                RoleId = model.RoleId,
                IsMain = model.IsMain,
                Rate = model.Rate
            };
        }

        private async Task MapCollections(EmploymentShortInfoViewModel model)
        {
            model.Roles = await _roleService.GetRolesWithIdsAsync();
            model.Locations = await _locationService.GetLocationsWithIdsAsync();
            model.Departments = await _departmentService.GetDepartmentsWithIdsAsync();
        }
    }
}
