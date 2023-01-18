using RestaurantsApplication.Data.Entities;

namespace RestaurantsApplication.DTOs.ShiftDTOs
{
    public class ShiftWithIdDTO
    {
        public int ShiftId { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeCode { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
    }
}
