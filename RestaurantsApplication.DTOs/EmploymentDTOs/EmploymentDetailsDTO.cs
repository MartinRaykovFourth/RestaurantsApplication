namespace RestaurantsApplication.DTOs.EmploymentDTOs
{
    public class EmploymentDetailsDTO
    {
        public int Id { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string DepartmentName { get; set; }
        public string LocationName { get; set; }
        public string RoleName { get; set; }
        public bool IsMain { get; set; }
        public decimal Rate { get; set; }
    }
}
