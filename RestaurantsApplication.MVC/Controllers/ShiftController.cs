using Microsoft.AspNetCore.Mvc;
using RestaurantsApplication.DTOs.ShiftDTOs;
using RestaurantsApplication.MVC.Models.Location;
using RestaurantsApplication.MVC.Models.Shift;
using RestaurantsApplication.Services.Contracts;
using static RestaurantsApplication.MVC.Messages.ErrorMessages;

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

            int overlappedShifts = (await _shiftService.GetOverlappedShiftsAsync(filterDate, locationCode))
                .Count(s => model.NotCompletedShifts
                .Any(ns => ns.ShiftId == s.ShiftId) == false);

            model.NotCompletedCount = model.NotCompletedShifts.Count() + overlappedShifts;

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> ManageNotCompletedShifts(DateTime date, string locationCode)
        {
            var model = new ManageShiftsViewModel()
            {
                LocationCode = locationCode
            };

            model.OverlappedShifts = await MapOverlappedShifts(date, locationCode);

            model.NotCompletedShifts = (await MapNotCompletedShifts(date, locationCode))
                .Where(s => model.OverlappedShifts.Any(os => os.ShiftId == s.ShiftId) == false)
                .ToList();

            model.Date = date;

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ManageNotCompletedShifts(ManageShiftsViewModel model)
        {
            if (model.NotCompletedShifts.Count > 0)
            {
                foreach (var shift in model.NotCompletedShifts)
                {
                    await _shiftService.ApplyRoleAsync(shift.ShiftId, shift.RoleId, model.LocationCode);
                }

                model.NotCompletedShifts.Clear();
            }

            if (!ModelState.IsValid)
            {
                await MapCollections(model);

                return View(model);
            }

            CheckForErrors(model);

            if (ModelState.ErrorCount > 0)
            {
                await MapCollections(model);

                return View(model);
            }

            foreach (var shift in model.OverlappedShifts.Where(s => s.ForRemoval))
            {
                await _shiftService.RemoveShiftAsync(shift.ShiftId);
            }
            model.OverlappedShifts.RemoveAll(s => s.ForRemoval);

            var dtos = model.OverlappedShifts
                .Select(s => new OverlappedShiftDTO
                {
                    ShiftId = s.ShiftId,
                    Start = s.Start,
                    BreakStart = s.BreakStart,
                    BreakEnd = s.BreakEnd,
                    End = s.End,
                    RoleId = s.RoleId
                });

            await _shiftService.ResolveOverlappedShiftsAsync(dtos);
            
            var date = model.Date.Date;
            var locationCode = model.LocationCode;

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
                    ShiftId = d.ShiftId,
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
                    ShiftId = d.ShiftId,
                    EmployeeName = d.EmployeeName,
                    Start = d.Start,
                    End = d.End,
                    HoursWorked = d.HoursWorked,
                    Cost = d.Cost
                }),

                Locations = locationModels
            };
        }

        private async Task<List<NotCompletedShiftViewModel>> MapNotCompletedShifts(DateTime date, string locationCode)
        {
            var notCompletedDTOs = await _shiftService.GetNotCompletedShiftsAsync(date, locationCode);

            var models = notCompletedDTOs
                .Select(d => new NotCompletedShiftViewModel
                {
                    EmployeeCode = d.EmployeeCode,
                    EmployeeName = d.EmployeeName,
                    Start = d.Start,
                    End = d.End,
                    ShiftId = d.ShiftId,
                })
                .ToList();

            foreach (var shift in models)
            {
                shift.AvailableRoles = await _roleService.GetAvailableRolesAsync(date, shift.EmployeeCode);
            }

            return models;
        }

        private async Task<List<OverlappedShiftViewModel>> MapOverlappedShifts(DateTime date, string locationCode)
        {
            var overlappedDTOs = await _shiftService.GetOverlappedShiftsAsync(date, locationCode);

            var models = overlappedDTOs
                .Select(d => new OverlappedShiftViewModel
                {
                    EmployeeCode = d.EmployeeCode,
                    EmployeeName = d.EmployeeName,
                    Start = d.Start,
                    End = d.End,
                    ShiftId = d.ShiftId,
                    BreakEnd = d.BreakEnd,
                    BreakStart = d.BreakStart,
                    RoleId = d.RoleId
                })
                .OrderBy(s => s.EmployeeName)
                .ToList();

            foreach (var shift in models)
            {
                shift.AvailableRoles = await _roleService.GetAvailableRolesAsync(date, shift.EmployeeCode);
            }

            return models;
        }

        private async Task MapCollections(ManageShiftsViewModel model)
        {
            foreach (var shift in model.OverlappedShifts)
            {
                shift.AvailableRoles = await _roleService.GetAvailableRolesAsync(model.Date, shift.EmployeeCode);
            }

            foreach (var shift in model.NotCompletedShifts)
            {
                shift.AvailableRoles = await _roleService.GetAvailableRolesAsync(model.Date, shift.EmployeeCode);
            }
        }

        private void CheckForErrors(ManageShiftsViewModel model)
        {
            if (model.OverlappedShifts.Any(ShiftsStillOverlap(model)))
            {
                ModelState.AddModelError("", ShiftsStillOverlappedError);
            }

            if (model.OverlappedShifts
                .Any(s => s.Start.Value.Date != model.Date.Date
                || s.End.Value.Date != model.Date.Date
                || (s.BreakStart.HasValue ? s.BreakStart.Value.Date != model.Date.Date : false)
                || (s.BreakEnd.HasValue ? s.BreakEnd.Value.Date != model.Date.Date : false))
                )
            {
                ModelState.AddModelError("", DateChangedError);
            }

            for (int i = 0; i < model.OverlappedShifts.Count; i++)
            {
                var shift = model.OverlappedShifts[i];

                if (shift.ForRemoval)
                {
                    continue;
                }

                if (shift.Start >= shift.End)
                {
                    ModelState.AddModelError($"OverlappedShifts[{i}].Start", InvalidClockTimeError);
                    ModelState.AddModelError($"OverlappedShifts[{i}].End", InvalidClockTimeError);
                }

                if (shift.BreakStart.HasValue && shift.BreakEnd.HasValue == false)
                {
                    ModelState.AddModelError($"OverlappedShifts[{i}].BreakEnd", BreakStartWithoutEndError);
                }
                else if (shift.BreakEnd.HasValue && shift.BreakStart.HasValue == false)
                {
                    ModelState.AddModelError($"OverlappedShifts[{i}].BreakStart", BreakEndWithoutStartError);
                }
                else if (shift.BreakStart.HasValue && shift.BreakEnd.HasValue)
                {
                    if (shift.BreakStart >= shift.BreakEnd)
                    {
                        ModelState.AddModelError($"OverlappedShifts[{i}].BreakStart", InvalidClockTimeError);
                        ModelState.AddModelError($"OverlappedShifts[{i}].BreakEnd", InvalidClockTimeError);
                    }

                    if (shift.BreakStart < shift.Start)
                    {
                        ModelState.AddModelError($"OverlappedShifts[{i}].BreakStart", InvalidClockTimeError);
                        ModelState.AddModelError($"OverlappedShifts[{i}].Start", InvalidClockTimeError);
                    }

                    if (shift.BreakStart >= shift.End)
                    {
                        ModelState.AddModelError($"OverlappedShifts[{i}].BreakStart", InvalidClockTimeError);
                        ModelState.AddModelError($"OverlappedShifts[{i}].End", InvalidClockTimeError);
                    }

                    if (shift.BreakEnd < shift.Start)
                    {
                        ModelState.AddModelError($"OverlappedShifts[{i}].BreakEnd", InvalidClockTimeError);
                        ModelState.AddModelError($"OverlappedShifts[{i}].Start", InvalidClockTimeError);
                    }

                    if (shift.BreakEnd > shift.End)
                    {
                        ModelState.AddModelError($"OverlappedShifts[{i}].BreakEnd", InvalidClockTimeError);
                        ModelState.AddModelError($"OverlappedShifts[{i}].End", InvalidClockTimeError);
                    }
                }
            }
        }

        private Func<OverlappedShiftViewModel, bool> ShiftsStillOverlap(ManageShiftsViewModel model)
        {
            return s1 => model.OverlappedShifts
                                .Any(s2 => s1.EmployeeCode == s2.EmployeeCode
                                    && s2.ShiftId != s1.ShiftId
                                    && s1.ForRemoval == false
                                    && s2.ForRemoval == false
                                    && (Shift2StartsInsideShift1(s1, s2)
                                    || Shift1InsideShift2(s1, s2)
                                    || Shift2InsideShift1(s1, s2)
                                    || Shift1StartsInsideShift2(s1, s2)
                                    ));
        }

        private bool Shift1StartsInsideShift2(OverlappedShiftViewModel s1, OverlappedShiftViewModel s2)
        {
            return s1.Start.Value.TimeOfDay >= s2.Start.Value.TimeOfDay
                   && s1.End.Value.TimeOfDay >= s2.End.Value.TimeOfDay
                   && s1.Start.Value.TimeOfDay < s2.End.Value.TimeOfDay;
        }

        private bool Shift2InsideShift1(OverlappedShiftViewModel s1, OverlappedShiftViewModel s2)
        {
            return s1.Start.Value.TimeOfDay <= s2.Start.Value.TimeOfDay && s1.End.Value.TimeOfDay >= s2.End.Value.TimeOfDay;
        }

        private bool Shift1InsideShift2(OverlappedShiftViewModel s1, OverlappedShiftViewModel s2)
        {
            return s1.Start.Value.TimeOfDay >= s2.Start.Value.TimeOfDay && s1.End.Value.TimeOfDay <= s2.End.Value.TimeOfDay;
        }

        private bool Shift2StartsInsideShift1(OverlappedShiftViewModel s1, OverlappedShiftViewModel s2)
        {
            return s1.Start.Value.TimeOfDay <= s2.Start.Value.TimeOfDay
                   && s1.End.Value.TimeOfDay <= s2.End.Value.TimeOfDay
                   && s2.Start.Value.TimeOfDay < s1.End.Value.TimeOfDay;
        }
    }
}
