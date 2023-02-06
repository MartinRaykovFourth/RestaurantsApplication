using RestaurantsApplication.DTOs.DatabaseCopiesDTOs;
using RestaurantsApplication.DTOs.EmployeeDTOs;

namespace RestaurantsApplication.Repositories.Contracts
{
    public interface IEmployeeRepository
    {
        public void Add(EmployeeShortInfoDTO dto);
        public Task<IEnumerable<EmployeeWithIdDTO>> GetAllAsync();
        public Task<EmployeeWithIdDTO> GetByIdAsync(int employeeId);
        public Task<int> GetIdByCodeAsync(string employeeCode);
        public Task<EmployeeCopyDTO> GetByCodeWithIncludesAsync(string employeeCode);
        public Task EditAsync(EmployeeWithIdDTO dto);
        public Task DeleteAsync(int employeeId);
        public Task SaveChangesAsync();
    }
}
