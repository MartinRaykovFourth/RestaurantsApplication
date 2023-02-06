using RestaurantsApplication.Repositories.Contracts;
using RestaurantsApplication.Services.Contracts;

namespace RestaurantsApplication.Services.Services
{
    public class ValidatorService : IValidatorService
    {
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IEmploymentRepository _employmentRepository;

        public ValidatorService(IDepartmentRepository departmentRepository, 
            IEmployeeRepository employeeRepository, 
            IEmploymentRepository employmentRepository)
        {
            _departmentRepository = departmentRepository;
            _employeeRepository = employeeRepository;
            _employmentRepository = employmentRepository;
        }

        public async Task<bool> ValidateDepartmentAsync(int departmentId, int locationId)
        {
            var departments = await _departmentRepository.GetIdsByLocationAsync(locationId);

            return departments.Any(d => d == departmentId);
        }

        public async Task<int> ValidateEmployeeAsync(string employeeCode)
        {
            return await _employeeRepository.GetIdByCodeAsync(employeeCode);
        }

        public async Task<bool> CanEmploymentBeMainAsync(int employeeId)
        {
            return !await _employmentRepository.EmployeeHasMainEmploymentAsync(employeeId);
        }

        public async Task<bool> ValidateEmploymentRoleAsync(int roleId, int departmentId, int employeeId)
        {
           return !await _employmentRepository.EmploymentWithRoleAndDepartmentExist(roleId, departmentId, employeeId);
        }
    }
}
