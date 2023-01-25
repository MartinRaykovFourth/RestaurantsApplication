using RestaurantsApplication.Data;
using RestaurantsApplication.DTOs.LabourDTOs;
using RestaurantsApplication.DTOs.EmployeeDTOs;
using RestaurantsApplication.Services.Contracts;

namespace RestaurantsApplication.Services.Services
{
    public class LabourService : ILabourService
    {
        private readonly IShiftService _shiftService;
        public LabourService(RestaurantsContext context, IShiftService shiftService)
        {
            _shiftService = shiftService;
        }

        public async Task<LabourCostPerDayDTO> GetLabourCostAsync(DateTime date, string locationCode)
        {
            var shifts = await _shiftService.GetByDateAndLocationAsync(date.Date, locationCode);

            decimal? weeklyCost = 0;

            DateTime startOfWeek = date
                .AddDays(1 - (date.DayOfWeek == 0 ? 7 : (int)date.DayOfWeek));

            for (int i = 0; i < 7; i++)
            {
                var currentDay = startOfWeek.AddDays(i).Date;
                if (DateTime.Now.Date < currentDay.Date)
                {
                    break;
                }
                var daily = (await _shiftService.GetEmployeesCostsByDateAsync(currentDay, locationCode)).Sum();

                weeklyCost += daily;
            }

            var labourDTO = new LabourCostPerDayDTO
            {
                Employees = shifts.Select(s => new EmployeeWithCostDTO
                {
                    Name = s.EmployeeName,
                    Cost = s.Cost,
                })
                .ToList(),
                WeeklyCost = weeklyCost
            };

            return labourDTO;
        }
    }
}
