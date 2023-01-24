namespace RestaurantsApplication.DTOs.ShiftDTOs
{
    public class OverlappedShiftDTO
    {
        public int ShiftId { get; set; }
        public string EmployeeName { get; set; } = null!;
        public string EmployeeCode { get; set; } = null!;
        public DateTime? Start { get; set; }
        public DateTime? End { get; set; }
        public DateTime? BreakStart { get; set; }
        public DateTime? BreakEnd { get; set; }
        public int? RoleId { get; set; }
    }
}
