using RestaurantsApplication.DTOs.DepartmentDTOs;

namespace RestaurantsApplication.Services.Contracts
{
    public interface IDepartmentService
    {
        public Task<DepartmentFullInfoDTO> GetByIdAsync(int departmentId);
        public Task<IEnumerable<DepartmentWithIdDTO>> GetDepartmentsWithIdsAsync();
        public Task AddAsync(DepartmentShortInfoDTO dto);
        public Task<IEnumerable<DepartmentWithLocationDTO>> GetAllAsync();
        public Task EditAsync(DepartmentFullInfoDTO dto);
    }
}
