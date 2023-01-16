using RestaurantsApplication.DTOs.RequestDTOs;

namespace RestaurantsApplication.Services.Contracts
{
    public interface ISubmitionService
    {
        public Task<IEnumerable<RequestShortInfoDTO>> GetRequestsByDate(DateTime date);
    }
}