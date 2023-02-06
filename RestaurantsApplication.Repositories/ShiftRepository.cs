using Microsoft.EntityFrameworkCore;
using RestaurantsApplication.Data;
using RestaurantsApplication.Data.Entities;
using RestaurantsApplication.DTOs.DatabaseCopiesDTOs;
using RestaurantsApplication.DTOs.ShiftDTOs;
using RestaurantsApplication.Repositories.Contracts;

namespace RestaurantsApplication.Repositories
{
    public class ShiftRepository : IShiftRepository
    {
        private readonly RestaurantsContext _context;

        public ShiftRepository(RestaurantsContext context)
        {
            _context = context;
        }

        public async Task<List<ShiftWithEmployeeDTO>> GetShiftsAsync(DateTime date, string locationCode)
        {
            return await _context.Shifts
               .Where(s => s.Start.Value.Date == date.Date
               && s.Employee.Employments
                   .Any(e => e.StartDate.Date <= date.Date
                       && (e.EndDate.HasValue ? e.EndDate.Value.Date >= date.Date : true)
                       && e.Department.Location.Code == locationCode))
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
                    EmployeeId = s.Employee.Id
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<ShiftWithIdDTO>> GetNotCompletedShiftsAsync(DateTime date, string locationCode)
        {
            return await _context.Shifts
                .Where(s => s.Start!.Value.Date == date.Date
                && s.Employee.Employments
                     .Any(e => e.StartDate.Date <= date.Date
                         && (e.EndDate.HasValue ? e.EndDate.Value.Date >= date.Date : true)
                         && e.Department.Location.Code == locationCode)
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

        public async Task<IEnumerable<ShiftWithTimesDTO>> GetOverlappedShiftsAsync(DateTime date, string locationCode)
        {
            return await _context.Shifts
                .Where(s1 => s1.Start!.Value.Date == date.Date
                && s1.Employee.Employments
                     .Any(e => e.StartDate.Date <= date.Date
                         && (e.EndDate.HasValue ? e.EndDate.Value.Date >= date.Date : true)
                         && e.Department.Location.Code == locationCode)
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
                .Select(s => new ShiftWithTimesDTO
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

        public bool CheckEqualShiftsAsync(ShiftCopyDTO dto)
        {
            return _context.Shifts
                  .Any(s => s.Employee.Code == dto.Employee.Code
                       && s.Start.HasValue && dto.Start.HasValue && s.Start == dto.Start
                       && s.End.HasValue && dto.End.HasValue && s.End == dto.End
                       && (
                       (s.BreakStart.HasValue && dto.BreakStart.HasValue && s.BreakStart == dto.BreakStart)
                       || (s.BreakStart.HasValue == false && dto.BreakStart.HasValue == false)
                       )
                       && (
                       (s.BreakEnd.HasValue && dto.BreakEnd.HasValue && s.BreakEnd == dto.BreakEnd)
                       || (s.BreakEnd.HasValue == false && dto.BreakEnd.HasValue == false)
                       ));
        }

        public async Task<ShiftWithTimesDTO> GetShiftWithoutBreakStartAsync(string employeeCode, DateTime? breakStart)
        {
            return await _context.Shifts
                            .Where(s =>
                            s.Employee.Code == employeeCode
                            && s.BreakStart.HasValue == false
                            && s.Start <= breakStart
                            && s.End > breakStart)
                            .Select(s => new ShiftWithTimesDTO
                            {
                                BreakEnd = s.BreakEnd,
                                BreakStart = s.BreakStart,
                                End = s.End,
                                Start = s.Start,
                                ShiftId = s.Id
                            })
                            .FirstOrDefaultAsync();
        }

        public async Task<ShiftWithTimesDTO> GetShiftWithoutBreakEndAsync(string employeeCode, DateTime? breakEnd)
        {
            return await _context.Shifts 
                            .Where(s =>
                            s.Employee.Code == employeeCode
                            && s.BreakEnd.HasValue == false
                            && s.Start < breakEnd
                            && s.End >= breakEnd)
                            .Select(s => new ShiftWithTimesDTO
                            {
                                BreakEnd = s.BreakEnd,
                                BreakStart = s.BreakStart,
                                End = s.End,
                                Start = s.Start,
                                ShiftId = s.Id
                            })
                            .FirstOrDefaultAsync();
        }

        public void AddRange(List<ShiftCopyDTO> shifts)
        {
            foreach (var s in shifts)
            {
                Shift shift = new Shift
                {
                    Start = s.Start,
                    End = s.End,
                    EmployeeId = s.EmployeeId,
                    BreakEnd = s.BreakEnd,
                    BreakStart = s.BreakStart,
                    DepartmentId = s.DepartmentId,
                    RoleId = s.RoleId,
                };

                _context.Shifts.Add(shift);
            }
        }

        public async Task EditShiftRoleAsync(int shiftId, int roleId, string locationCode)
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
        }

        public async Task EditShiftClocksAsync(ShiftWithTimesDTO dto)
        {
            var shift = await _context.Shifts.FindAsync(dto.ShiftId);

            shift.Start = dto.Start;
            shift.BreakStart = dto.BreakStart;
            shift.BreakEnd = dto.BreakEnd;
            shift.End = dto.End;
            shift.RoleId = dto.RoleId;
        }

        public async Task DeleteAsync(int shiftId)
        {
            var shift = await _context.Shifts.FindAsync(shiftId);

            _context.Shifts.Remove(shift!);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

    }
}
