namespace RestaurantsApplication.Api.ApiDTOs
{
    public class ClockRecordsDTO
    {
        public DateTime Date { get; set; }
        public string LocationCode { get; set; }
        public RecordDTO[] Records { get; set; }
    }
}
