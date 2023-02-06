using RestaurantsApplication.DTOs.EmploymentDTOs;

namespace RestaurantsApplication.Services.Contracts
{
    public interface IEmploymentService
    {
        public Task AddAsync(EmploymentShortInfoDTO dto);
        public Task<IEnumerable<EmploymentDetailsDTO>> GetEmploymentsForEmployeeAsync(int employeeId);
        public Task<EmploymentWithIdDTO> GetByIdAsync(int employmentId);
        public Task EditAsync(EmploymentWithIdDTO dto);
        public Task<int> GetMainEmploymentIdAsync(int employeeId);
        public Task<int> DeleteAsync(int employmentId);
    }
}
