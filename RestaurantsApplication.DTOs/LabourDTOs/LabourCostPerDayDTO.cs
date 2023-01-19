using RestaurantsApplication.DTOs.EmployeeDTOs;

namespace RestaurantsApplication.DTOs.LabourDTOs
{
    public class LabourCostPerDayDTO
    {
        public List<EmployeeWithCostDTO> Employees { get; set; } = new List<EmployeeWithCostDTO>();
        public decimal? Total => Employees.Where(e => e.Cost != null).Sum(e => e.Cost);
        public decimal? WeeklyCost { get; set; }
    }
}
