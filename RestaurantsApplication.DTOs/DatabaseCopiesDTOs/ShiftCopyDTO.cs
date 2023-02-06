namespace RestaurantsApplication.DTOs.DatabaseCopiesDTOs
{
    public class ShiftCopyDTO
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public EmployeeCopyDTO Employee { get; set; }
        public int? DepartmentId { get; set; }
        public DepartmentCopyDTO Department { get; set; }
        public int? RoleId { get; set; }
        public RoleCopyDTO Role { get; set; }
        public DateTime? Start { get; set; }
        public DateTime? End { get; set; }
        public DateTime? BreakStart { get; set; }
        public DateTime? BreakEnd { get; set; }
    }
}
