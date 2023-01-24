namespace RestaurantsApplication.MVC.Models.Shift
{
    public class ManageShiftsViewModel
    {
        public List<OverlappedShiftViewModel> OverlappedShifts { get; set; } = new List<OverlappedShiftViewModel>();
        public List<NotCompletedShiftViewModel> NotCompletedShifts { get; set; } = new List<NotCompletedShiftViewModel>();
        public string LocationCode { get; set; }
        public DateTime Date { get; set; }
    }
}
