using RestaurantsApplication.DTOs.LocationDTOs;

namespace RestaurantsApplication.Services.Contracts
{
    public interface ILocationService
    {
        public Task<IEnumerable<LocationWithIdDTO>> GetLocationsWithIdsAsync();
        public Task AddAsync(LocationShortInfoDTO dto);
        public Task<IEnumerable<LocationWithCodeDTO>> GetAllAsync();
        public Task<LocationWithCodeDTO> GetByIdAsync(int locationId);
        public Task EditAsync(LocationWithCodeDTO dto);
        public Task DeleteAsync(int locationId);
        public Task<LocationShortInfoDTO> GetByCodeAsync(string code);
    }
}
