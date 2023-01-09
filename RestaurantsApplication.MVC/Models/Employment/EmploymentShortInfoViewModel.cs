using static RestaurantsApplication.MVC.Messages.ErrorMessages;
using System.ComponentModel.DataAnnotations;
using RestaurantsApplication.DTOs.LocationDTOs;
using RestaurantsApplication.DTOs.RoleDTOs;
using RestaurantsApplication.DTOs.DepartmentDTOs;

namespace RestaurantsApplication.MVC.Models.Employment
{
    public class EmploymentShortInfoViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = StartDateRequiredError)]
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int DepartmentId { get; set; }
        public int LocationId { get; set; }
        public int RoleId { get; set; }

        [Required(ErrorMessage = CodeRequiredError)]
        [StringLength(8, MinimumLength = 8, ErrorMessage = CodeError)]
        public string EmployeeCode { get; set; }
        public int EmployeeId { get; set; }
        public bool IsMain { get; set; }
        [Required]
        public decimal Rate { get; set; }

        public IEnumerable<LocationWithIdDTO> Locations { get; set; } = new List<LocationWithIdDTO>();
        public IEnumerable<RoleWithIdDTO> Roles { get; set; } = new List<RoleWithIdDTO>();
        public IEnumerable<DepartmentWithIdDTO> Departments { get; set; } = new List<DepartmentWithIdDTO>();
    }
}
