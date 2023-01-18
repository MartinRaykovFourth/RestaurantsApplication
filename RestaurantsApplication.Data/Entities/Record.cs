using RestaurantsApplication.Data.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestaurantsApplication.Data.Entities
{
    public class Record
    {
        public int Id { get; set; }
        public string EmployeeCode { get; set; }
        [ForeignKey(nameof(EmployeeCode))]
        public Employee Employee { get; set; }
        public ClockStatus ClockStatus { get; set; }
        public DateTime ClockValue { get; set; }
        public int RequestId { get; set; }
        [ForeignKey(nameof(RequestId))]
        public Request Request { get; set; }
    }
}
