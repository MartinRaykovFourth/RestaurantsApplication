namespace RestaurantsApplication.DTOs.EmploymentDTOs
{
    public class EmploymentShortInfoDTO
    {
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int DepartmentId { get; set; }
        public int LocationId { get; set; }
        public int RoleId { get; set; }
        public int EmployeeId { get; set; }
        public bool IsMain { get; set; }
        public decimal Rate { get; set; }
    }
}
