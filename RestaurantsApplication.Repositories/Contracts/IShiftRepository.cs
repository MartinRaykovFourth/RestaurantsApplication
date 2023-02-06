using RestaurantsApplication.DTOs.DatabaseCopiesDTOs;
using RestaurantsApplication.DTOs.ShiftDTOs;

namespace RestaurantsApplication.Repositories.Contracts
{
    public interface IShiftRepository
    {
        public Task<List<ShiftWithEmployeeDTO>> GetShiftsAsync(DateTime date, string locationCode);
        public Task<IEnumerable<ShiftWithIdDTO>> GetNotCompletedShiftsAsync(DateTime date, string locationCode);
        public Task<IEnumerable<ShiftWithTimesDTO>> GetOverlappedShiftsAsync(DateTime date, string locationCode);
        public bool CheckEqualShiftsAsync(ShiftCopyDTO dto);
        public Task<ShiftWithTimesDTO> GetShiftWithoutBreakStartAsync(string employeeCode, DateTime? breakStart);
        public Task<ShiftWithTimesDTO> GetShiftWithoutBreakEndAsync(string employeeCode, DateTime? breakEnd);
        public void AddRange(List<ShiftCopyDTO> shifts);
        public Task EditShiftRoleAsync(int shiftId, int roleId, string locationCode);
        public Task EditShiftClocksAsync(ShiftWithTimesDTO dto);
        public Task DeleteAsync(int shiftId);
        public Task SaveChangesAsync();
    }
}
