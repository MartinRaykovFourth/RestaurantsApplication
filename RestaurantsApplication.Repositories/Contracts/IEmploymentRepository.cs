using RestaurantsApplication.DTOs.EmploymentDTOs;

namespace RestaurantsApplication.Repositories.Contracts
{
    public interface IEmploymentRepository
    {
        public void Add(EmploymentShortInfoDTO dto);
        public Task<IEnumerable<EmploymentDetailsDTO>> GetEmploymentsForEmployeeAsync(int employeeId);
        public Task<EmploymentWithIdDTO> GetByIdAsync(int employmentId);
        public Task<bool> EmployeeHasMainEmploymentAsync(int employeeId);
        public Task<bool> EmploymentWithRoleAndDepartmentExist(int roleId, int departmentId, int employeeId);
        public Task EditAsync(EmploymentWithIdDTO dto);
        public Task<int> GetMainEmploymentId(int employeeId);
        public Task<decimal> GetEmploymentRateAsync(int employeeId, int? roleId, int? departmentId);
        public Task DeleteAsync(int employmentId);
        public Task SaveChangesAsync();
    }
}
