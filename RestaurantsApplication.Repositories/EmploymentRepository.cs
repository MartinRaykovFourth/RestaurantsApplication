using Microsoft.EntityFrameworkCore;
using RestaurantsApplication.Data;
using RestaurantsApplication.Data.Entities;
using RestaurantsApplication.DTOs.EmploymentDTOs;
using RestaurantsApplication.Repositories.Contracts;

namespace RestaurantsApplication.Repositories
{
    public class EmploymentRepository : IEmploymentRepository
    {
        private readonly RestaurantsContext _context;

        public EmploymentRepository(RestaurantsContext context)
        {
            _context = context;
        }

        public void Add(EmploymentShortInfoDTO dto)
        {
            var employment = new Employment()
            {
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                DepartmentId = dto.DepartmentId,
                EmployeeId = dto.EmployeeId,
                RoleId = dto.RoleId,
                IsMain = dto.IsMain,
                Rate = dto.Rate
            };

            _context.Employments.Add(employment);
        }

        public async Task<IEnumerable<EmploymentDetailsDTO>> GetEmploymentsForEmployeeAsync(int employeeId)
        {
            return await _context.Employments
                .Where(e => e.EmployeeId == employeeId)
                .Select(e => new EmploymentDetailsDTO
                {
                    Id = e.Id,
                    StartDate = e.StartDate.ToString("dd/MM/yyyy"),
                    EndDate = e.EndDate != null ? e.EndDate.Value.ToString("dd/MM/yyyy") : "N/A",
                    DepartmentName = e.Department.Name,
                    RoleName = e.Role.Name,
                    LocationName = e.Department.Location.Name,
                    IsMain = e.IsMain,
                    Rate = e.Rate

                })
            .ToListAsync();
        }

        public async Task<EmploymentWithIdDTO> GetByIdAsync(int employmentId)
        {
            return await _context.Employments
                .Where(e => e.Id == employmentId)
                .Select(e => new EmploymentWithIdDTO
                {
                    DepartmentId = e.DepartmentId,
                    EmployeeId = e.EmployeeId,
                    EndDate = e.EndDate,
                    Id = e.Id,
                    IsMain = e.IsMain,
                    LocationId = e.Department.LocationId,
                    Rate = e.Rate,
                    RoleId = e.RoleId,
                    StartDate = e.StartDate,
                    EmployeeCode = e.Employee.Code
                })
            .SingleAsync();
        }

        public async Task<bool> EmployeeHasMainEmploymentAsync(int employeeId)
        {
            var employment = await _context.Employments
                .Where(e => e.EmployeeId == employeeId && e.IsMain)
                .SingleOrDefaultAsync();

            return employment != null;
        }

        public async Task<bool> EmploymentWithRoleAndDepartmentExist(int roleId, int departmentId, int employeeId)
        {
            var employment = await _context.Employments
                .Where(e => e.EmployeeId == employeeId 
                && e.RoleId == roleId 
                && e.DepartmentId == departmentId)
                .SingleOrDefaultAsync();

            return employment != null;
        }

        public async Task EditAsync(EmploymentWithIdDTO dto)
        {
            var employment = await _context.Employments.FindAsync(dto.Id);

            employment.IsMain = dto.IsMain;
            employment.RoleId = dto.RoleId;
            employment.Rate = dto.Rate;
            employment.StartDate = dto.StartDate;
            employment.EndDate = dto.EndDate;
            employment.DepartmentId = dto.DepartmentId;
        }

        public async Task<int> GetMainEmploymentId(int employeeId)
        {
            return await _context.Employments
                .Where(e => e.EmployeeId == employeeId && e.IsMain)
                .Select(e => e.Id)
            .SingleAsync();
        }

        public async Task<decimal> GetEmploymentRateAsync(int employeeId, int? roleId, int? departmentId)
        {
            return await _context.Employments
                     .Where(e =>
                     e.EmployeeId == employeeId
                     && e.RoleId == roleId
                     && e.DepartmentId == departmentId)
                     .Select(e => e.Rate)
                     .SingleAsync();
        }

        public async Task DeleteAsync(int employmentId)
        {
            var employment = await _context.Employments.FindAsync(employmentId);

            employment.EndDate = DateTime.Now;
            employment.IsDeleted = true;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
