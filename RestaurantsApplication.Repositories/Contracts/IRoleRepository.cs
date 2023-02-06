using RestaurantsApplication.DTOs.RoleDTOs;

namespace RestaurantsApplication.Repositories.Contracts
{
    public interface IRoleRepository
    {
        public Task<IEnumerable<RoleWithIdDTO>> GetAllWtihIdAsync();
        public Task<List<RoleWithIdDTO>> GetAvailableRolesAsync(DateTime date, string employeeCode);

    }
}
