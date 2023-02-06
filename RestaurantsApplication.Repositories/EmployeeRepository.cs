using Microsoft.EntityFrameworkCore;
using RestaurantsApplication.Data;
using RestaurantsApplication.Data.Entities;
using RestaurantsApplication.DTOs.DatabaseCopiesDTOs;
using RestaurantsApplication.DTOs.EmployeeDTOs;
using RestaurantsApplication.Repositories.Contracts;

namespace RestaurantsApplication.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly RestaurantsContext _context;

        public EmployeeRepository(RestaurantsContext context)
        {
            _context = context;
        }

        public void Add(EmployeeShortInfoDTO dto)
        {
            var employee = new Employee
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Code = dto.Code
            };

            _context.Employees.Add(employee);
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
                .SingleAsync();
        }

        public async Task<int> GetIdByCodeAsync(string employeeCode)
        {
            return await _context.Employees
                 .Where(e => e.Code == employeeCode)
                 .Select(e => e.Id)
                 .SingleOrDefaultAsync();
        }

        public async Task<EmployeeCopyDTO> GetByCodeWithIncludesAsync(string employeeCode)
        {
           var employee = await _context.Employees
                .Include(e => e.Employments)
                .ThenInclude(e => e.Department)
                .ThenInclude(d => d.Location)
                .Where(e => e.Code == employeeCode)
                .SingleAsync();

            var dto = new EmployeeCopyDTO
            {
                Id = employee.Id,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                Code = employee.Code,
                Employments = employee.Employments
                .Select(e => new EmploymentCopyDTO
                {
                    DepartmentId = e.DepartmentId,
                    RoleId = e.RoleId,
                    Department = new DepartmentCopyDTO
                    {
                        Location = new LocationCopyDTO
                        {
                            Code = e.Department.Location.Code
                        }
                    }
                })
                .ToList()
            };

            return dto;
        }

        public async Task EditAsync(EmployeeWithIdDTO dto)
        {
            var employee = await _context.Employees
                .Where(e => e.Id == dto.Id)
                .SingleAsync();

            employee.FirstName = dto.FirstName;
            employee.LastName = dto.LastName;
            employee.Code = dto.Code;
        }

        public async Task DeleteAsync(int employeeId)
        {
            var employee = await _context.Employees
                .Include(e => e.Employments)
                .SingleAsync(e => e.Id == employeeId);

            foreach (var employment in
                employee.Employments
                .Where(e => !e.IsDeleted)
                .ToList())
            {
                employment.EndDate = DateTime.Now;
                employment.IsDeleted = true;
            }

            employee.IsDeleted = true;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
