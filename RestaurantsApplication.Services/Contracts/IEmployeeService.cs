using RestaurantsApplication.DTOs.EmployeeDTOs;
using RestaurantsApplication.DTOs.EmploymentDTOs;

namespace RestaurantsApplication.Services.Contracts
{
    public interface IEmployeeService
    {
        public Task AddEmployeeAsync(EmployeeShortInfoDTO dto);
        public Task<IEnumerable<EmployeeWithIdDTO>> GetAllAsync();
        public Task<EmployeeWithIdDTO> GetByIdAsync(int employeeId);
        public Task EditAsync(EmployeeWithIdDTO dto);
    }
}
