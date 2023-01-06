using Microsoft.EntityFrameworkCore;
using RestaurantsApplication.Data;
using RestaurantsApplication.Data.Entities;
using RestaurantsApplication.DTOs.LocationDTOs;
using RestaurantsApplication.Services.Contracts;

namespace RestaurantsApplication.Services.Services
{
    public class LocationService : ILocationService
    {
        private readonly RestaurantsContext _context;

        public LocationService(RestaurantsContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<LocationWithIdDTO>> GetLocationsWithIdsAsync()
        {
            return await _context.Locations
                .Select(l => new LocationWithIdDTO
                {
                    Id = l.Id,
                    Name = l.Name
                })
                .ToListAsync();
        }

        public async Task AddLocationAsync(LocationShortInfoDTO dto)
        {
            var location = new Location()
            {
                Name = dto.Name,
                Code = dto.Code
            };

            _context.Locations.Add(location);
            await _context.SaveChangesAsync();
        }
    }
}
