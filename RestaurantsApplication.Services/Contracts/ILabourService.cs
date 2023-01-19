using RestaurantsApplication.DTOs.LabourDTOs;

namespace RestaurantsApplication.Services.Contracts
{
    public interface ILabourService
    {
        public Task<LabourCostPerDayDTO> GetLabourCostAsync(DateTime date, string locationCode);
    }
}
