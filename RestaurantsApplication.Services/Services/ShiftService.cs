using Microsoft.EntityFrameworkCore;
using RestaurantsApplication.Data;
using RestaurantsApplication.DTOs.ShiftDTOs;
using RestaurantsApplication.Services.Contracts;

namespace RestaurantsApplication.Services.Services
{
    public class ShiftService : IShiftService
    {
        private readonly RestaurantsContext _context;

        public ShiftService(RestaurantsContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ShiftShortInfoDTO>> GetByDateAndLocationAsync(DateTime date, string locationCode)
        {
            List<ShiftWithEmployeeDTO> shifts = await GetShifts(date, locationCode);

            CalculateHoursWorkedAndCost(shifts);

            return shifts.Select(s => new ShiftShortInfoDTO
            {
                ShiftId = s.ShiftId,
                EmployeeName = s.EmployeeName,
                Start = s.Start,
                End = s.End,
                HoursWorked = s.HoursWorked,
                Cost = s.Cost
            });
        }

        public async Task<IEnumerable<ShiftWithIdDTO>> GetNotCompletedShiftsAsync(DateTime date, string locationCode)
        {
            return await _context.Shifts
                .Where(s => s.Start!.Value.Date == date.Date
                && s.Employee.Employments
                     .Any(e => e.StartDate.Date <= date.Date
                         && (e.EndDate.HasValue ? e.EndDate.Value.Date >= date.Date : true)
                         && e.Department.Location.Code == locationCode
                         && e.IsDeleted == false)
                && s.RoleId == null
                && s.DepartmentId == null)
                .Select(s => new ShiftWithIdDTO
                {
                    ShiftId = s.Id,
                    EmployeeName = $"{s.Employee.FirstName} {s.Employee.LastName}",
                    Start = s.Start!.Value,
                    End = s.End!.Value,
                    EmployeeCode = s.Employee.Code
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<OverlappedShiftDTO>> GetOverlappedShiftsAsync(DateTime date, string locationCode)
        {
            return await _context.Shifts
                .Where(s1 => s1.Start!.Value.Date == date.Date
                && s1.Employee.Employments
                     .Any(e => e.StartDate.Date <= date.Date
                         && (e.EndDate.HasValue ? e.EndDate.Value.Date >= date.Date : true)
                         && e.Department.Location.Code == locationCode
                         && e.IsDeleted == false)
                && _context.Shifts
                     .Any(s2 =>
                          s2.Start!.Value.Date == s1.Start.Value.Date
                          && s2.EmployeeId == s1.EmployeeId
                          && s2.Id != s1.Id
                          && (
                          (s1.Start.Value.TimeOfDay <= s2.Start.Value.TimeOfDay
                          && s1.End.Value.TimeOfDay <= s2.End.Value.TimeOfDay
                          && s2.Start.Value.TimeOfDay < s1.End.Value.TimeOfDay)
                          || (s1.Start.Value.TimeOfDay >= s2.Start.Value.TimeOfDay && s1.End.Value.TimeOfDay <= s2.End.Value.TimeOfDay)
                          || (s1.Start.Value.TimeOfDay <= s2.Start.Value.TimeOfDay && s1.End.Value.TimeOfDay >= s2.End.Value.TimeOfDay)
                          || (s1.Start.Value.TimeOfDay >= s2.Start.Value.TimeOfDay
                          && s1.End.Value.TimeOfDay >= s2.End.Value.TimeOfDay
                          && s1.Start.Value.TimeOfDay < s2.End.Value.TimeOfDay)
                          )))
                .Select(s => new OverlappedShiftDTO
                {
                    ShiftId = s.Id,
                    Start = s.Start,
                    End = s.End,
                    BreakStart = s.BreakStart,
                    BreakEnd = s.BreakEnd,
                    EmployeeCode = s.Employee.Code,
                    EmployeeName = $"{s.Employee.FirstName} {s.Employee.LastName}",
                    RoleId = s.RoleId
                })
               .ToListAsync();
        }

        public async Task ApplyRoleAsync(int shiftId, int roleId, string locationCode)
        {
            var shift = await _context.Shifts
                .Include(s => s.Employee)
                .ThenInclude(e => e.Employments)
                .ThenInclude(e => e.Department)
                .ThenInclude(d => d.Location)
                .Where(s => s.Id == shiftId)
                .SingleAsync();

            shift.RoleId = roleId;

            shift.DepartmentId = shift.Employee.Employments
                .Where(e =>
                e.RoleId == roleId
                && e.Department.Location.Code == locationCode
                && e.IsDeleted == false)
                .Select(e => e.DepartmentId)
                .Single();

            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<decimal?>> GetEmployeesCostsByDateAsync(DateTime date, string locationCode)
        {
            List<ShiftWithEmployeeDTO> shifts = await GetShifts(date, locationCode);

            CalculateHoursWorkedAndCost(shifts);

            return shifts
                .Where(s => s.Cost != null)
                .Select(s => s.Cost);

        }

        public async Task RemoveShiftAsync(int shiftId)
        {
            var shift = await _context.Shifts.FindAsync(shiftId);

            _context.Shifts.Remove(shift!);

            await _context.SaveChangesAsync();
        }

        public async Task ResolveOverlappedShiftsAsync(IEnumerable<OverlappedShiftDTO> shifts)
        {
            foreach (var s in shifts)
            {
                var shift = await _context.Shifts.FindAsync(s.ShiftId);

                shift.Start = s.Start;
                shift.BreakStart = s.BreakStart;
                shift.BreakEnd = s.BreakEnd;
                shift.End = s.End;
                shift.RoleId = s.RoleId;
            }

            await _context.SaveChangesAsync();
        }

        private async Task<List<ShiftWithEmployeeDTO>> GetShifts(DateTime date, string locationCode)
        {
            return await _context.Shifts
               .Where(s => s.Start.Value.Date == date.Date
               && s.Employee.Employments
                   .Any(e => e.StartDate.Date <= date.Date
                       && (e.EndDate.HasValue ? e.EndDate.Value.Date >= date.Date : true)
                       && e.Department.Location.Code == locationCode
                       && e.IsDeleted == false))
                .Select(s => new ShiftWithEmployeeDTO
                {
                    ShiftId = s.Id,
                    EmployeeName = s.Employee.FirstName + " " + s.Employee.LastName,
                    Start = s.Start!.Value,
                    End = s.End!.Value,
                    BreakStart = s.BreakStart.HasValue ? s.BreakStart.Value : null,
                    BreakEnd = s.BreakEnd.HasValue ? s.BreakEnd.Value : null,
                    RoleId = s.RoleId,
                    DepartmentId = s.DepartmentId,
                    Employee = s.Employee
                })
                .ToListAsync();
        }

        private void CalculateHoursWorkedAndCost(List<ShiftWithEmployeeDTO> shifts)
        {
            foreach (var shift in shifts)
            {
                shift.HoursWorked = shift.BreakStart.HasValue && shift.BreakEnd.HasValue ?
                    shift.End.TimeOfDay - shift.Start.TimeOfDay
                    -
                    (shift.BreakEnd.Value.TimeOfDay - shift.BreakStart.Value.TimeOfDay) :
                    shift.End.TimeOfDay - shift.Start.TimeOfDay;

                if (shift.RoleId != null && shift.DepartmentId != null)
                {
                    var rate = _context.Employments
                    .Where(e =>
                    e.EmployeeId == shift.Employee.Id
                    && e.RoleId == shift.RoleId
                    && e.DepartmentId == shift.DepartmentId)
                    .Select(e => e.Rate)
                    .Single();

                    shift.Cost = rate * shift.HoursWorked.Hours;
                }
            }
        }

    }
}
