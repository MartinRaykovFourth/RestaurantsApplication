using RestaurantsApplication.DTOs.LocationDTOs;
using RestaurantsApplication.Repositories.Contracts;
using RestaurantsApplication.Services.Contracts;

namespace RestaurantsApplication.Services.Services
{
    public class LocationService : ILocationService
    {
        private readonly ILocationRepository _locationRepo;

        public LocationService(ILocationRepository locationRepo)
        {
            _locationRepo = locationRepo;
        }

        public async Task<IEnumerable<LocationWithIdDTO>> GetLocationsWithIdsAsync()
        {
            return await _locationRepo.GetAllWithIdAsync();
        }

        public async Task AddAsync(LocationShortInfoDTO dto)
        {
            _locationRepo.Add(dto);

            await _locationRepo.SaveChangesAsync();
        }

        public async Task<IEnumerable<LocationWithCodeDTO>> GetAllAsync()
        {
            return await _locationRepo.GetAllWithCodeAsync();
        }

        public async Task<LocationWithCodeDTO> GetByIdAsync(int locationId)
        {
            return await _locationRepo.GetByIdAsync(locationId);
        }

        public async Task EditAsync(LocationWithCodeDTO dto)
        {
            await _locationRepo.EditAsync(dto);

            await _locationRepo.SaveChangesAsync();
        }

        public async Task DeleteAsync(int locationId)
        {
            await _locationRepo.DeleteAsync(locationId);

            await _locationRepo.SaveChangesAsync();
        }

    }
}
