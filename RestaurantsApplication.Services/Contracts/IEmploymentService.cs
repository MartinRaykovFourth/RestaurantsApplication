using RestaurantsApplication.DTOs.EmploymentDTOs;

namespace RestaurantsApplication.Services.Contracts
{
    public interface IEmploymentService
    {
        public Task AddAsync(EmploymentShortInfoDTO dto);
    }
}
