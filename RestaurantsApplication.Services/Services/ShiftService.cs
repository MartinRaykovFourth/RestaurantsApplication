using RestaurantsApplication.DTOs.ShiftDTOs;
using RestaurantsApplication.Repositories.Contracts;
using RestaurantsApplication.Services.Contracts;

namespace RestaurantsApplication.Services.Services
{
    public class ShiftService : IShiftService
    {
        private readonly IShiftRepository _shiftRepository;
        private readonly IEmploymentRepository _employmentRepository;

        public ShiftService(IShiftRepository shiftRepository, IEmploymentRepository employmentRepository)
        {
            _shiftRepository = shiftRepository;
            _employmentRepository = employmentRepository;
        }

        public async Task<IEnumerable<ShiftShortInfoDTO>> GetByDateAndLocationAsync(DateTime date, string locationCode)
        {
            List<ShiftWithEmployeeDTO> shifts = await _shiftRepository.GetShiftsAsync(date, locationCode);

            await CalculateHoursWorkedAndCost(shifts);

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
            return await _shiftRepository.GetNotCompletedShiftsAsync(date, locationCode);
        }

        public async Task<IEnumerable<ShiftWithTimesDTO>> GetOverlappedShiftsAsync(DateTime date, string locationCode)
        {
            return await _shiftRepository.GetOverlappedShiftsAsync(date, locationCode);
        }

        public async Task ApplyRoleAsync(int shiftId, int roleId, string locationCode)
        {
            await _shiftRepository.EditShiftRoleAsync(shiftId, roleId, locationCode);

            await _shiftRepository.SaveChangesAsync();
        }

        public async Task<IEnumerable<decimal?>> GetEmployeesCostsByDateAsync(DateTime date, string locationCode)
        {
            List<ShiftWithEmployeeDTO> shifts = await _shiftRepository.GetShiftsAsync(date, locationCode);

            await CalculateHoursWorkedAndCost(shifts);

            return shifts
                .Where(s => s.Cost != null)
                .Select(s => s.Cost);
        }

        public async Task RemoveShiftAsync(int shiftId)
        {
            await _shiftRepository.DeleteAsync(shiftId);

            await _shiftRepository.SaveChangesAsync();
        }

        public async Task ResolveOverlappedShiftsAsync(IEnumerable<ShiftWithTimesDTO> shifts)
        {
            foreach (var s in shifts)
            {
                await _shiftRepository.EditShiftClocksAsync(s);
            }

            await _shiftRepository.SaveChangesAsync();
        }

        private async Task CalculateHoursWorkedAndCost(List<ShiftWithEmployeeDTO> shifts)
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
                    var rate = await _employmentRepository.GetEmploymentRateAsync(shift.EmployeeId, shift.RoleId, shift.DepartmentId);

                    shift.Cost = rate * shift.HoursWorked.Hours;
                }
            }
        }

    }
}
