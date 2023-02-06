namespace RestaurantsApplication.DTOs.DatabaseCopiesDTOs
{
    public class DepartmentCopyDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int LocationId { get; set; }
        public LocationCopyDTO Location { get; set; }
    }
}
