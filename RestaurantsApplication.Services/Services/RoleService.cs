using RestaurantsApplication.DTOs.RoleDTOs;
using RestaurantsApplication.Repositories.Contracts;
using RestaurantsApplication.Services.Contracts;

namespace RestaurantsApplication.Services.Services
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _roleRepo;

        public RoleService(IRoleRepository _roleRepo)
        {
            this._roleRepo = _roleRepo;
        }

        public async Task<IEnumerable<RoleWithIdDTO>> GetRolesWithIdsAsync()
        {
            return await _roleRepo.GetAllWtihIdAsync();
        }

        public async Task<List<RoleWithIdDTO>> GetAvailableRolesAsync(DateTime date, string employeeCode)
        {
            return await _roleRepo.GetAvailableRolesAsync(date, employeeCode);
        }
    }
}
