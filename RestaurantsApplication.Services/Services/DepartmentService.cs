using RestaurantsApplication.DTOs.DepartmentDTOs;
using RestaurantsApplication.Repositories.Contracts;
using RestaurantsApplication.Services.Contracts;

namespace RestaurantsApplication.Services.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IDepartmentRepository _departmentRepo;

        public DepartmentService(IDepartmentRepository departmentRepo)
        {
            _departmentRepo = departmentRepo;
        }

        public async Task<IEnumerable<DepartmentWithIdDTO>> GetDepartmentsWithIdsAsync()
        {
            return await _departmentRepo.GetAllWithIdsAsync();
        }

        public async Task AddAsync(DepartmentShortInfoDTO dto)
        {
            _departmentRepo.Add(dto);

            await _departmentRepo.SaveChangesAsync();
        }

        public async Task<IEnumerable<DepartmentWithLocationDTO>> GetAllAsync()
        {
            return await _departmentRepo.GetAllWithLocationAsync();
        }

        public async Task<DepartmentFullInfoDTO> GetByIdAsync(int departmentId)
        {
            return await _departmentRepo.GetByIdAsync(departmentId);
        }

        public async Task EditAsync(DepartmentFullInfoDTO dto)
        {
            await _departmentRepo.EditAsync(dto);

            await _departmentRepo.SaveChangesAsync();
        }

        public async Task DeleteAsync(int departmentId)
        {
            _departmentRepo.DeleteAsync(departmentId);

            await _departmentRepo.SaveChangesAsync();
        }
    }
}
