using RestaurantsApplication.DTOs.EmployeeDTOs;
using RestaurantsApplication.MVC.Models.Location;

namespace RestaurantsApplication.MVC.Models.Labour
{
    public class LabourCostPerDayViewModel
    {
        public List<EmployeeWithCostDTO> Employees { get; set; } 
        public decimal? Total { get; set; }
        public decimal? WeeklyCost { get; set;}
        public IEnumerable<LocationShortInfoViewModel> Locations { get; set; } = new List<LocationShortInfoViewModel>();
    }
}
