using Microsoft.EntityFrameworkCore;
using RestaurantsApplication.Data;
using RestaurantsApplication.Data.Entities;
using RestaurantsApplication.DTOs.DepartmentDTOs;
using RestaurantsApplication.Services.Contracts;

namespace RestaurantsApplication.Services.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly RestaurantsContext _context;

        public DepartmentService(RestaurantsContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<DepartmentWithIdDTO>> GetDepartmentsWithIdsAsync()
        {
            return await _context.Departments
                .Select(d => new DepartmentWithIdDTO
                {
                    Id = d.Id,
                    Name = d.Name
                })
                .ToListAsync();
        }

        public async Task AddDepartmentAsync(DepartmentShortInfoDTO dto)
        {
            var department = new Department()
            {
                Name = dto.Name,
                LocationId = dto.LocationId
            };

            _context.Departments.Add(department);
            await _context.SaveChangesAsync();
        }
    }
}
