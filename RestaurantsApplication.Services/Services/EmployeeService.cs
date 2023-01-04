using RestaurantsApplication.Data;
using RestaurantsApplication.Data.Entities;
using RestaurantsApplication.DTOs.EmployeeDTOs;
using RestaurantsApplication.Services.Contracts;

namespace RestaurantsApplication.Services.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly RestaurantsContext _context;

        public EmployeeService(RestaurantsContext context)
        {
            _context = context;
        }

        public async Task AddEmployee(EmployeeShortInfoDTO dto)
        {
            var employee = new Employee
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Code = dto.Code
            };

            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();
        }
    }
}