using Microsoft.EntityFrameworkCore;
using RestaurantsApplication.Data;
using RestaurantsApplication.Services.Contracts;

namespace RestaurantsApplication.Services.Services
{
    public class ValidatorService : IValidatorService
    {
        private readonly RestaurantsContext _context;

        public ValidatorService(RestaurantsContext context)
        {
            _context = context;
        }

        public async Task<bool> ValidateDepartmentAsync(int departmentId, int locationId)
        {
            var location = await _context.Locations
                .Include(l => l.Departments)
                .Where(l => l.Id == locationId)
                .SingleOrDefaultAsync();

            return location.Departments.Any(d => d.Id == departmentId);
        }

        public async Task<int> ValidateEmployeeAsync(string employeeCode)
        {
           return await _context.Employees
                  .Where(e => e.Code == employeeCode)
                  .Select(e => e.Id)
                  .SingleOrDefaultAsync();
        }

        public async Task<bool> CanEmploymentBeMainAsync(int employeeId)
        {
            var employee = await _context.Employees 
                .Include(e => e.Employments) // moje da gurmi tuka null
                .Where(e => e.Id == employeeId)
                .SingleOrDefaultAsync();

            return !employee.Employments.Any(e => e.IsMain);
        }
    }
}
