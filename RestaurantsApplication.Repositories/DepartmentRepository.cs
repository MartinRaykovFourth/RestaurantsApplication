using Microsoft.EntityFrameworkCore;
using RestaurantsApplication.Data;
using RestaurantsApplication.Data.Entities;
using RestaurantsApplication.DTOs.DepartmentDTOs;
using RestaurantsApplication.Repositories.Contracts;

namespace RestaurantsApplication.Repositories
{
    public class DepartmentRepository : IDepartmentRepository
    {
        private readonly RestaurantsContext _context;

        public DepartmentRepository(RestaurantsContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<DepartmentWithIdDTO>> GetAllWithIdsAsync()
        {
            return await _context.Departments
                .Select(d => new DepartmentWithIdDTO
                {
                    Id = d.Id,
                    Name = d.Name
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<DepartmentWithLocationDTO>> GetAllWithLocationAsync()
        {
            return await _context.Departments
                .Select(d => new DepartmentWithLocationDTO()
                {
                    Id = d.Id,
                    Name = d.Name,
                    LocationName = d.Location.Name
                })
                .ToListAsync();
        }

        public void Add(DepartmentShortInfoDTO dto)
        {
            var department = new Department()
            {
                Name = dto.Name,
                LocationId = dto.LocationId
            };

            _context.Departments.Add(department);
        }

        public async Task<DepartmentFullInfoDTO> GetByIdAsync(int departmentId)
        {
            return await _context.Departments
                .Where(d => d.Id == departmentId)
                .Select(d => new DepartmentFullInfoDTO()
                {
                    Id = d.Id,
                    Name = d.Name,
                    LocationId = d.LocationId
                })
                .SingleAsync();
        }

        public async Task<IEnumerable<int>> GetIdsByLocationAsync(int locationId)
        {
            return await _context.Departments
                .Where(d => d.LocationId == locationId)
                .Select(d => d.Id)
                .ToListAsync();
        }

        public async Task EditAsync(DepartmentFullInfoDTO dto)
        {
            var department = await _context.Departments
                .Where(d => d.Id == dto.Id)
                .SingleAsync();

            department.Name = dto.Name;
            department.LocationId = dto.LocationId;
        }

        public async Task DeleteAsync(int departmentId)
        {
            var department = await _context.Departments.FindAsync(departmentId);

            var employments = await _context.Employments
                .Where(e => e.DepartmentId == departmentId)
                .ToListAsync();

            foreach (var e in employments)
            {
                e.EndDate = DateTime.Now;
                e.IsDeleted = true;
            }

            department.IsDeleted = true;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

    }
}

