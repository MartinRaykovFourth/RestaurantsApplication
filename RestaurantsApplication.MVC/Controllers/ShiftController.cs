using Microsoft.AspNetCore.Mvc;
using RestaurantsApplication.DTOs.ShiftDTOs;
using RestaurantsApplication.MVC.Models.Location;
using RestaurantsApplication.MVC.Models.Shift;
using RestaurantsApplication.Services.Contracts;

namespace RestaurantsApplication.MVC.Controllers
{
    public class ShiftController : Controller
    {
        private readonly ILocationService _locationService;
        private readonly IShiftService _shiftService;
        private readonly IRoleService _roleService;

        public ShiftController(ILocationService locationService, IShiftService shiftServoce, IRoleService roleService)
        {
            _locationService = locationService;
            _shiftService = shiftServoce;
            _roleService = roleService;
        }

        public async Task<IActionResult> ViewAll(string locationCode, DateTime? date)
        {
            DateTime filterDate;
            if (date.HasValue)
            {
                filterDate = date.Value;
            }
            else
            {
                filterDate = DateTime.Now.Date;
            }

            ViewBag.Date = filterDate.Date.ToString("yyyy-MM-dd");

            var locationDTOs = await _locationService.GetAllAsync();

            var locationModels = locationDTOs
                .Select(d => new LocationShortInfoViewModel
                {
                    Name = d.Name,
                    Code = d.Code
                });

            if (string.IsNullOrEmpty(locationCode))
            {
                var modelWithLocations = new ShiftsWithLocationsViewModel
                {
                    Locations = locationModels
                };
                return View(modelWithLocations);
            }

            ViewBag.LocationCode = locationCode;

            var shiftDTOs = await _shiftService.GetByDateAndLocationAsync(filterDate, locationCode);

            var model = MapModel(locationModels, shiftDTOs);

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> ManageNotCompletedShifts(DateTime date, string locationCode)
        {
            var dtos = await _shiftService.GetNotCompletedShiftsAsync(date, locationCode);

            var models = dtos
                .Select(d => new NotCompletedShiftViewModel
            {
                EmployeeCode = d.EmployeeCode,
                EmployeeName = d.EmployeeName,
                Start = d.Start,
                End = d.End,
                ShiftId = d.ShiftId,
                LocationCode = locationCode
            })
                .ToList();

            foreach (var model in models)
            {
                model.AvailableRoles = await _roleService.GetAvailableRolesAsync(date, model.EmployeeCode);
            }

            return View(models);
        }

        [HttpPost]
        public async Task<IActionResult> ManageNotCompletedShifts(List<NotCompletedShiftViewModel> models)
        {
            foreach (var model in models)
            {
                await _shiftService.ApplyRoleAsync(model.ShiftId,model.RoleId,model.LocationCode);
            }

            var date = models[0].Start.Date;
            var locationCode = models[0].LocationCode;

            return RedirectToAction(nameof(ViewAll), new { locationCode, date });
        }
        private static ShiftsWithLocationsViewModel MapModel(IEnumerable<LocationShortInfoViewModel> locationModels, IEnumerable<ShiftShortInfoDTO> shiftDTOs)
        {
            return new ShiftsWithLocationsViewModel()
            {
                CompletedShifts = shiftDTOs
                .Where(d => d.Cost != null)
                .Select(d => new ShiftShortInfoViewModel
                {
                    EmployeeName = d.EmployeeName,
                    Start = d.Start,
                    End = d.End,
                    HoursWorked = d.HoursWorked,
                    Cost = d.Cost
                }),

                NotCompletedShifts = shiftDTOs
                .Where(d => d.Cost == null)
                .Select(d => new ShiftShortInfoViewModel
                {
                    EmployeeName = d.EmployeeName,
                    Start = d.Start,
                    End = d.End,
                    HoursWorked = d.HoursWorked,
                    Cost = d.Cost
                }),

                Locations = locationModels
            };
        }
    }
}
