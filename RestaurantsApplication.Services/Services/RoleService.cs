using Microsoft.EntityFrameworkCore;
using RestaurantsApplication.Data;
using RestaurantsApplication.DTOs.RoleDTOs;
using RestaurantsApplication.Services.Contracts;

namespace RestaurantsApplication.Services.Services
{
    public class RoleService : IRoleService
    {
        private readonly RestaurantsContext _context;

        public RoleService(RestaurantsContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<RoleWithIdDTO>> GetRolesWithIdsAsync()
        {
            return await _context.Roles
                .Select(r => new RoleWithIdDTO
                {
                    Id = r.Id,
                    Name = r.Name
                })
                .ToListAsync();
        }

        public async Task<List<RoleWithIdDTO>> GetAvailableRolesAsync(DateTime date, string employeeCode)
        {
            return await _context.Employments
                .Where(e => e.Employee.Code == employeeCode
                && e.StartDate.Date <= date.Date
                && (e.EndDate.HasValue ? e.EndDate.Value.Date >= date.Date : true)
                && e.IsDeleted == false)
                .Select(e => new RoleWithIdDTO
                {
                    Id = e.RoleId,
                    Name = e.Role.Name
                })
                .ToListAsync();
        }
    }
}
