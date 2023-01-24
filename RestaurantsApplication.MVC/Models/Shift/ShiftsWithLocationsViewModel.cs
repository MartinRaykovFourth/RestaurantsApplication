using RestaurantsApplication.MVC.Models.Location;

namespace RestaurantsApplication.MVC.Models.Shift
{
    public class ShiftsWithLocationsViewModel
    {
        public IEnumerable<ShiftShortInfoViewModel> CompletedShifts { get; set; }
        public IEnumerable<ShiftShortInfoViewModel> NotCompletedShifts { get; set; }
        public int NotCompletedCount { get; set; }
        public IEnumerable<LocationShortInfoViewModel> Locations { get; set; } = new List<LocationShortInfoViewModel>();
    }
}
