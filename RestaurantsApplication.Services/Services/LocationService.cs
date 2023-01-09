using Microsoft.EntityFrameworkCore;
using RestaurantsApplication.Data;
using RestaurantsApplication.Data.Entities;
using RestaurantsApplication.DTOs.DepartmentDTOs;
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

        public async Task AddAsync(LocationShortInfoDTO dto)
        {
            var location = new Location()
            {
                Name = dto.Name,
                Code = dto.Code
            };

            _context.Locations.Add(location);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<LocationWithCodeDTO>> GetAllAsync()
        {
            return await _context.Locations
                .Select(l => new LocationWithCodeDTO
                {
                    Id = l.Id,
                    Name = l.Name,
                    Code = l.Code
                })
                .ToListAsync();
        }

        public async Task<LocationWithCodeDTO> GetByIdAsync(int locationId)
        {
            return await _context.Locations
                .Where(l => l.Id == locationId)
                .Select(l => new LocationWithCodeDTO()
                {
                    Id = l.Id,
                    Name = l.Name,
                    Code = l.Code
                })
                .SingleOrDefaultAsync();
        }

        public async Task EditAsync(LocationWithCodeDTO dto)
        {
            var location = await _context.Locations
                .Where(l => l.Id == dto.Id)
                .SingleAsync();

            location.Name = dto.Name;
            location.Code = dto.Code;

            await _context.SaveChangesAsync();
        }
    }
}
