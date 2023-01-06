using Microsoft.EntityFrameworkCore;
using RestaurantsApplication.Data;
using RestaurantsApplication.Data.Entities;
using RestaurantsApplication.DTOs.DepartmentDTOs;
using RestaurantsApplication.DTOs.EmployeeDTOs;
using RestaurantsApplication.DTOs.EmploymentDTOs;
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

        public async Task AddEmployeeAsync(EmployeeShortInfoDTO dto)
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

        public async Task<IEnumerable<EmployeeWithIdDTO>> GetAllAsync()
        {
            return await _context.Employees
                .Select(e => new EmployeeWithIdDTO()
                {
                    Id = e.Id,
                    FirstName = e.FirstName,
                    LastName = e.LastName,
                    Code = e.Code
                })
                .ToListAsync();
        }

        public async Task<EmployeeWithIdDTO> GetByIdAsync(int employeeId)
        {
            return await _context.Employees
                .Where(e => e.Id == employeeId)
                .Select(e => new EmployeeWithIdDTO()
                {
                    Id = e.Id,
                    FirstName = e.FirstName,
                    LastName = e.LastName,
                    Code = e.Code
                })
                .SingleOrDefaultAsync();
        }
        public async Task EditAsync(EmployeeWithIdDTO dto)
        {
            var employee = await _context.Employees
                .Where(e => e.Id == dto.Id)
                .SingleAsync();

            employee.FirstName = dto.FirstName;
            employee.LastName = dto.LastName;
            employee.Code = dto.Code;

            await _context.SaveChangesAsync();
        }

    }
}