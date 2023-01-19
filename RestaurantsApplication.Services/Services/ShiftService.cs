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
           .Where(s => s.Employee.Employments.Any(e =>
           e.StartDate.Date <= date.Date
           && (e.EndDate.HasValue ? e.EndDate.Value.Date >= date.Date : true)
           && e.Department.Location.Code == locationCode
           && e.IsDeleted == false)
           && s.Start.Value.Date == date.Date
           && s.RoleId == null
           && s.DepartmentId == null)
           .Select(s => new ShiftWithIdDTO
           {
               ShiftId = s.Id,
               EmployeeName = s.Employee.FirstName + " " + s.Employee.LastName,
               Start = s.Start.Value,
               End = s.End.Value,
               EmployeeCode = s.Employee.Code
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

        private async Task<List<ShiftWithEmployeeDTO>> GetShifts(DateTime date, string locationCode)
        {
            return await _context.Shifts
                .Where(s => s.Employee.Employments.Any(e =>
                e.StartDate.Date <= date.Date
                && (e.EndDate.HasValue ? e.EndDate.Value.Date >= date.Date : true)
                && e.Department.Location.Code == locationCode
                && e.IsDeleted == false)
                && s.Start.Value.Date == date.Date)
                .Select(s => new ShiftWithEmployeeDTO
                {
                    EmployeeName = s.Employee.FirstName + " " + s.Employee.LastName,
                    Start = s.Start.Value,
                    End = s.End.Value,
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
