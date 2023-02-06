using Microsoft.EntityFrameworkCore;
using RestaurantsApplication.Data;
using RestaurantsApplication.DTOs.RequestDTOs;
using RestaurantsApplication.Repositories.Contracts;

namespace RestaurantsApplication.Repositories
{
    public class SubmitionRepository : ISubmitionRepository
    {
        private readonly RestaurantsContext _context;

        public SubmitionRepository(RestaurantsContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<RequestShortInfoDTO>> GetAllByDateAsync(DateTime date)
        {
            return await _context.Requests
                .Where(r => r.Date.Date == date.Date)
                .Select(r => new RequestShortInfoDTO
                {
                    Date = r.Date,
                    LocationCode = r.LocationCode,
                    Status = r.Status,
                    FailMessage = r.FailMessage
                })
                .ToListAsync();
        }
    }
}
