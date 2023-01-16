namespace RestaurantsApplication.MVC.Models.Submition
{
    public class SubmitionShortInfoViewModel
    {
        public DateTime Date { get; set; }
        public string LocationCode { get; set; }
        public string Status { get; set; }
        public string? FailMessage { get; set; }
    }
}
