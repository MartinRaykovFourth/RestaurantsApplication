using RestaurantsApplication.DTOs.EmployeeDTOs;
using RestaurantsApplication.Repositories.Contracts;
using RestaurantsApplication.Services.Contracts;

namespace RestaurantsApplication.Services.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepo;

        public EmployeeService(IEmployeeRepository employeeRepo)
        {
            _employeeRepo = employeeRepo;
        }

        public async Task AddAsync(EmployeeShortInfoDTO dto)
        {
            _employeeRepo.Add(dto);

            await _employeeRepo.SaveChangesAsync();
        }

        public async Task<IEnumerable<EmployeeWithIdDTO>> GetAllAsync()
        {
            return await _employeeRepo.GetAllAsync();
        }

        public async Task<EmployeeWithIdDTO> GetByIdAsync(int employeeId)
        {
            return await _employeeRepo.GetByIdAsync(employeeId);
        }
        public async Task EditAsync(EmployeeWithIdDTO dto)
        {
            await _employeeRepo.EditAsync(dto);

            await _employeeRepo.SaveChangesAsync();
        }

        public async Task DeleteAsync(int employeeId)
        {
            await _employeeRepo.DeleteAsync(employeeId);

            await _employeeRepo.SaveChangesAsync();
        }
    }
}