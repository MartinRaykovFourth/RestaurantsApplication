using Microsoft.EntityFrameworkCore;
using RestaurantsApplication.Data;
using RestaurantsApplication.DTOs.RequestDTOs;
using RestaurantsApplication.Services.Contracts;

namespace RestaurantsApplication.Services.Services
{
    public class SubmitionService : ISubmitionService
    {
        private readonly RestaurantsContext _context;

        public SubmitionService(RestaurantsContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<RequestShortInfoDTO>> GetRequestsByDate(DateTime date)
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
