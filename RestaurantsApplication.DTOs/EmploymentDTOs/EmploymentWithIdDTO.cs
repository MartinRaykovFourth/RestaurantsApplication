namespace RestaurantsApplication.DTOs.EmploymentDTOs
{
	public class EmploymentWithIdDTO
	{
        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int DepartmentId { get; set; }
        public int LocationId { get; set; }
        public int RoleId { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeCode { get; set; }
        public bool IsMain { get; set; }
        public decimal Rate { get; set; }
    }
}
