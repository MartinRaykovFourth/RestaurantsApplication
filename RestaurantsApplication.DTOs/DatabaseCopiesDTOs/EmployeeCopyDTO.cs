namespace RestaurantsApplication.DTOs.DatabaseCopiesDTOs
{
    public class EmployeeCopyDTO
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Code { get; set; }
        public List<EmploymentCopyDTO> Employments { get; set; } = new List<EmploymentCopyDTO>();
    }
}
