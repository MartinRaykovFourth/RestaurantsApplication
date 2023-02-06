using RestaurantsApplication.DTOs.ShiftDTOs;

namespace RestaurantsApplication.Services.Contracts
{
    public interface IShiftService
    {
        public Task<IEnumerable<ShiftShortInfoDTO>> GetByDateAndLocationAsync(DateTime date, string locationCode);
        public Task<IEnumerable<ShiftWithIdDTO>> GetNotCompletedShiftsAsync(DateTime date, string locationCode);
        public Task ApplyRoleAsync(int shiftId, int roleId, string locationCode);
        public Task<IEnumerable<decimal?>> GetEmployeesCostsByDateAsync(DateTime date, string locationCode);
        public Task<IEnumerable<ShiftWithTimesDTO>> GetOverlappedShiftsAsync(DateTime date, string locationCode);
        public Task RemoveShiftAsync(int shiftId);
        public Task ResolveOverlappedShiftsAsync(IEnumerable<ShiftWithTimesDTO> shifts);
    }
}
