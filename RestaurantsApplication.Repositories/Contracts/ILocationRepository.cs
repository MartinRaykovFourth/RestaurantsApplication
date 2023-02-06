using RestaurantsApplication.DTOs.LocationDTOs;

namespace RestaurantsApplication.Repositories.Contracts
{
    public interface ILocationRepository
    {
        public Task<IEnumerable<LocationWithIdDTO>> GetAllWithIdAsync();
        public void Add(LocationShortInfoDTO dto);
        public Task<IEnumerable<LocationWithCodeDTO>> GetAllWithCodeAsync();
        public Task<LocationWithCodeDTO> GetByIdAsync(int locationId);
        public Task EditAsync(LocationWithCodeDTO dto);
        public Task DeleteAsync(int locationId);
        public Task SaveChangesAsync();
    }
}
