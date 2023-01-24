using RestaurantsApplication.DTOs.RoleDTOs;

namespace RestaurantsApplication.Services.Contracts
{
    public interface IRoleService
    {
        public Task<IEnumerable<RoleWithIdDTO>> GetRolesWithIdsAsync();
        public Task<List<RoleWithIdDTO>> GetAvailableRolesAsync(DateTime date, string employeeCode);
    }
}
