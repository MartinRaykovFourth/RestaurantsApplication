using System.ComponentModel.DataAnnotations.Schema;

namespace RestaurantsApplication.Data.Entities
{
    public class Request
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string LocationCode { get; set; }
        [ForeignKey(nameof(LocationCode))]
        public Location Location { get; set; }
        public string Status { get; set; }
        public string? FailMessage { get; set; }
        public IEnumerable<Record> Records { get; set; }
    }
}
