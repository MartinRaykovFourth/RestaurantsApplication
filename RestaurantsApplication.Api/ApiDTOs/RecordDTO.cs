using RestaurantsApplication.DTOs.Enums;

namespace RestaurantsApplication.Api.ApiDTOs
{
    public class RecordDTO
    {
        public string EmployeeCode { get; set; }
        public ClockStatus ClockStatus { get; set; }
        public DateTime ClockValue { get; set; }
    }
}
