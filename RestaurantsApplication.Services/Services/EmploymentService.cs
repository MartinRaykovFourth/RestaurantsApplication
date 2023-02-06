using RestaurantsApplication.DTOs.EmploymentDTOs;
using RestaurantsApplication.Repositories.Contracts;
using RestaurantsApplication.Services.Contracts;

namespace RestaurantsApplication.Services.Services
{
    public class EmploymentService : IEmploymentService
    {
        private readonly IEmploymentRepository _employmentRepository;

        public EmploymentService(IEmploymentRepository repo)
        {
            _employmentRepository = repo;
        }

        public async Task AddAsync(EmploymentShortInfoDTO dto)
        {
            _employmentRepository.Add(dto);

            await _employmentRepository.SaveChangesAsync();
        }

        public async Task<IEnumerable<EmploymentDetailsDTO>> GetEmploymentsForEmployeeAsync(int employeeId)
        {
            return await _employmentRepository.GetEmploymentsForEmployeeAsync(employeeId);
        }

        public async Task<EmploymentWithIdDTO> GetByIdAsync(int employmentId)
        {
            return await _employmentRepository.GetByIdAsync(employmentId);
        }

        public async Task EditAsync(EmploymentWithIdDTO dto)
        {
            await _employmentRepository.EditAsync(dto);

            await _employmentRepository.SaveChangesAsync();
        }

        public async Task<int> GetMainEmploymentIdAsync(int employeeId)
        {
            return await _employmentRepository.GetMainEmploymentId(employeeId);
        }

        public async Task<int> DeleteAsync(int employmentId)
        {
            var employment = await _employmentRepository.GetByIdAsync(employmentId);

            if (employment.IsMain)
                return -1;

            await _employmentRepository.DeleteAsync(employmentId);

            await _employmentRepository.SaveChangesAsync();

            return employment.EmployeeId;
        }
    }
}
