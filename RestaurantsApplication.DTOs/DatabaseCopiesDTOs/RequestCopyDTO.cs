namespace RestaurantsApplication.DTOs.DatabaseCopiesDTOs
{
    public class RequestCopyDTO
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string LocationCode { get; set; }
        public LocationCopyDTO Location { get; set; }
        public string Status { get; set; }
        public string? FailMessage { get; set; }
        public List<RecordCopyDTO> Records { get; set; }
    }
}
