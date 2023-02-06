namespace RestaurantsApplication.DTOs.DatabaseCopiesDTOs
{
    public class LocationCopyDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set;}
        public List<DepartmentCopyDTO> Departments { get; set; } = new List<DepartmentCopyDTO>();
    }
}