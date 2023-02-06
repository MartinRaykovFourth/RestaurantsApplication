using RestaurantsApplication.DTOs.DepartmentDTOs;

namespace RestaurantsApplication.Repositories.Contracts
{
    public interface IDepartmentRepository
    {
        public Task<IEnumerable<DepartmentWithIdDTO>> GetAllWithIdsAsync();
        public Task<IEnumerable<DepartmentWithLocationDTO>> GetAllWithLocationAsync();
        public void Add(DepartmentShortInfoDTO dto);
        public Task<DepartmentFullInfoDTO> GetByIdAsync(int departmentId);
        public Task<IEnumerable<int>> GetIdsByLocationAsync(int locationId);
        public Task EditAsync(DepartmentFullInfoDTO dto);
        public Task DeleteAsync(int departmentId);
        public Task SaveChangesAsync();

    }
}
