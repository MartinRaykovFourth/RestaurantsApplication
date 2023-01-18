using RestaurantsApplication.DTOs.RoleDTOs;

namespace RestaurantsApplication.MVC.Models.Shift
{
	public class NotCompletedShiftViewModel
	{
		public int ShiftId { get; set; }
		public string EmployeeName { get; set; }
		public string EmployeeCode { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
		public int RoleId { get; set; }
		public string LocationCode { get; set; }
		public IEnumerable<RoleWithIdDTO> AvailableRoles { get; set; } = new List<RoleWithIdDTO>();
    }
}
