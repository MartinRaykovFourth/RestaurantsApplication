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
        private readonly IDepartmentService _departmentService;
        public LocationService(RestaurantsContext context, IDepartmentService departmentService)
        {
            _context = context;
            _departmentService = departmentService;
        }

        public async Task<IEnumerable<LocationWithIdDTO>> GetLocationsWithIdsAsync()
        {
            return await _context.Locations
                .Where(l => l.IsDeleted == false)
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
                .Where(l => l.IsDeleted == false)
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

        public async Task DeleteAsync(int locationId)
        {
            var location = await _context.Locations.FindAsync(locationId);

            var departments = await _context.Departments
                .Where(d => d.LocationId == locationId && d.IsDeleted == false)
                .Select(d => d.Id)
                .ToListAsync();

            foreach (var d in departments)
            {
                await _departmentService.DeleteAsync(d);
            }

            location.IsDeleted = true;

            await _context.SaveChangesAsync();
        }

    }
}
