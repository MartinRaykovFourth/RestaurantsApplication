using RestaurantsApplication.DTOs.DepartmentDTOs;

namespace RestaurantsApplication.Services.Contracts
{
    public interface IDepartmentService
    {
        public Task<IEnumerable<DepartmentWithIdDTO>> GetDepartmentsWithIdsAsync();
        public Task AddDepartmentAsync(DepartmentShortInfoDTO dto);
    }
}
