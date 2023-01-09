using RestaurantsApplication.DTOs.EmployeeDTOs;

namespace RestaurantsApplication.Services.Contracts
{
    public interface IEmployeeService
    {
        public Task AddAsync(EmployeeShortInfoDTO dto);
        public Task<IEnumerable<EmployeeWithIdDTO>> GetAllAsync();
        public Task<EmployeeWithIdDTO> GetByIdAsync(int employeeId);
        public Task EditAsync(EmployeeWithIdDTO dto);
        public Task DeleteAsync(int employeeId);
    }
}
