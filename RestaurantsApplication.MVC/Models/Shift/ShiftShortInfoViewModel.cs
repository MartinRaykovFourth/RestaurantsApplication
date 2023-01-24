namespace RestaurantsApplication.MVC.Models.Shift
{
    public class ShiftShortInfoViewModel
    {
        public int ShiftId { get; set; }
        public string EmployeeName { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public TimeSpan HoursWorked { get; set; }
        public decimal? Cost { get; set; }
    }
}
