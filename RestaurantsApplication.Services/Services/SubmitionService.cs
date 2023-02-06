using RestaurantsApplication.DTOs.RequestDTOs;
using RestaurantsApplication.Repositories.Contracts;
using RestaurantsApplication.Services.Contracts;

namespace RestaurantsApplication.Services.Services
{
    public class SubmitionService : ISubmitionService
    {
        private readonly ISubmitionRepository _repo;

        public SubmitionService(ISubmitionRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<RequestShortInfoDTO>> GetRequestsByDateAsync(DateTime date)
        {
            return await _repo.GetAllByDateAsync(date);
        }
    }
}
