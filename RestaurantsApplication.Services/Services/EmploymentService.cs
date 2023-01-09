using Microsoft.EntityFrameworkCore;
using RestaurantsApplication.Data;
using RestaurantsApplication.Data.Entities;
using RestaurantsApplication.DTOs.EmploymentDTOs;
using RestaurantsApplication.Services.Contracts;

namespace RestaurantsApplication.Services.Services
{
    public class EmploymentService : IEmploymentService
    {
        private readonly RestaurantsContext _context;

        public EmploymentService(RestaurantsContext context)
        {
            _context = context;
        }

        public async Task AddAsync(EmploymentShortInfoDTO dto)
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
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<EmploymentDetailsDTO>> GetEmploymentsForEmployeeAsync(int employeeId)
        {
            return await _context.Employments
                .Where(e => e.EmployeeId == employeeId && e.IsDeleted == false)
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
                .Where(e => e.Id == employmentId && e.IsDeleted == false)
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

        public async Task EditAsync(EmploymentWithIdDTO dto)
        {
            var employment = await _context.Employments.FindAsync(dto.Id);

            employment.IsMain = dto.IsMain;
            employment.RoleId = dto.RoleId;
            employment.Rate = dto.Rate;
            employment.StartDate = dto.StartDate;
            employment.EndDate = dto.EndDate;
            employment.DepartmentId = dto.DepartmentId;

            await _context.SaveChangesAsync();
        }

        public async Task<int> GetMainEmploymentId(int employeeId)
        {
            return await _context.Employments
                .Where(e => e.EmployeeId == employeeId && e.IsMain)
                .Select(e => e.Id)
                .SingleAsync();
        }

        public async Task<int> DeleteAsync(int employmentId)
        {
            var employment = await _context.Employments.FindAsync(employmentId);

            if (employment.IsMain)
                return -1;

            employment.EndDate = DateTime.Now;
            employment.IsDeleted = true;

            await _context.SaveChangesAsync();

            return employment.EmployeeId;
        }
    }
}
