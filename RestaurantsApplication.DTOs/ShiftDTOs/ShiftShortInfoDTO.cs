using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantsApplication.DTOs.ShiftDTOs
{
    public class ShiftShortInfoDTO
    {
        public string EmployeeName { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public TimeSpan HoursWorked { get; set; }
        public decimal? Cost { get; set; }
    }
}
