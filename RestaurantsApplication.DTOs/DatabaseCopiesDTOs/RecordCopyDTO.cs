using RestaurantsApplication.Data.Enums;

namespace RestaurantsApplication.DTOs.DatabaseCopiesDTOs
{
    public class RecordCopyDTO
    {
        public int Id { get; set; }
        public string EmployeeCode { get; set; }
        public EmployeeCopyDTO Employee { get; set; }
        public ClockStatus ClockStatus { get; set; }
        public DateTime ClockValue { get; set; }
        public int RequestId { get; set; }
        public RequestCopyDTO Request { get; set; }
    }
}
