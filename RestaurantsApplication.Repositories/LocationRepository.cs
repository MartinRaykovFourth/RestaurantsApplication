using Microsoft.EntityFrameworkCore;
using RestaurantsApplication.Data;
using RestaurantsApplication.Data.Entities;
using RestaurantsApplication.DTOs.LocationDTOs;
using RestaurantsApplication.Repositories.Contracts;

namespace RestaurantsApplication.Repositories
{
    public class LocationRepository : ILocationRepository
    {
        private readonly RestaurantsContext _context;
        private readonly DepartmentRepository _departmentRepository;

        public LocationRepository(RestaurantsContext context)
        {
            _context = context;
            _departmentRepository = new DepartmentRepository(_context);
        }

        public async Task<IEnumerable<LocationWithIdDTO>> GetAllWithIdAsync()
        {
            return await _context.Locations
                .Select(l => new LocationWithIdDTO
                {
                    Id = l.Id,
                    Name = l.Name
                })
                .ToListAsync();
        }

        public void Add(LocationShortInfoDTO dto)
        {
            var location = new Location()
            {
                Name = dto.Name,
                Code = dto.Code
            };

            _context.Locations.Add(location);
        }

        public async Task<IEnumerable<LocationWithCodeDTO>> GetAllWithCodeAsync()
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
            .SingleAsync();
        }

        public async Task EditAsync(LocationWithCodeDTO dto)
        {
            var location = await _context.Locations
                .Where(l => l.Id == dto.Id)
            .SingleAsync();

            location.Name = dto.Name;
            location.Code = dto.Code;
        }

        public async Task DeleteAsync(int locationId)
        {
            var location = await _context.Locations.FindAsync(locationId);

            var departments = await _context.Departments
                .Where(d => d.LocationId == locationId)
                .Select(d => d.Id)
                .ToListAsync();

            foreach (var d in departments)
            {
                await _departmentRepository.DeleteAsync(d);
            }

            location.IsDeleted = true;
        }

        public async Task SaveChangesAsync()
        {
           await _context.SaveChangesAsync();
        }
    }
}
