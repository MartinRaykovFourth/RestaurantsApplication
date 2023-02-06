namespace RestaurantsApplication.DTOs.DatabaseCopiesDTOs
{
    public class EmploymentCopyDTO
    {
        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int DepartmentId { get; set; }
        public DepartmentCopyDTO Department { get; set; }
        public int RoleId { get; set; }
        public RoleCopyDTO Role { get; set; }
        public int EmployeeId { get; set; }
        public EmployeeCopyDTO Employee { get; set; }
        public bool IsMain { get; set; }
        public decimal Rate { get; set; }

    }
}
