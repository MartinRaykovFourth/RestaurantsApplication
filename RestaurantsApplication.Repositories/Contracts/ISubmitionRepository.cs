using RestaurantsApplication.DTOs.RequestDTOs;

namespace RestaurantsApplication.Repositories.Contracts
{
    public interface ISubmitionRepository
    {
        public Task<IEnumerable<RequestShortInfoDTO>> GetAllByDateAsync(DateTime date);
    }
}
