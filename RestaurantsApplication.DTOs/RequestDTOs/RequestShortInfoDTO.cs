namespace RestaurantsApplication.DTOs.RequestDTOs
{
    public class RequestShortInfoDTO
    {
        public DateTime Date { get; set; }
        public string LocationCode { get; set; }
        public string Status { get; set; }
        public string? FailMessage { get; set; }
    }
}
