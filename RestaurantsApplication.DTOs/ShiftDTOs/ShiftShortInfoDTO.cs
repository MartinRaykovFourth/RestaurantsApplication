namespace RestaurantsApplication.DTOs.ShiftDTOs
{
    public class ShiftShortInfoDTO
    {
        public int ShiftId { get; set; }
        public string EmployeeName { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public TimeSpan HoursWorked { get; set; }
        public decimal? Cost { get; set; }
    }
}
