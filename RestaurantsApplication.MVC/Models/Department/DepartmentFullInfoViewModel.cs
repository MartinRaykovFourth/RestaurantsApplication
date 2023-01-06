using RestaurantsApplication.DTOs.LocationDTOs;
using System.ComponentModel.DataAnnotations;
using static RestaurantsApplication.MVC.Messages.ErrorMessages;

namespace RestaurantsApplication.MVC.Models.Department
{
    public class DepartmentFullInfoViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = DepartmentNameRequiredError)]
        [MaxLength(50)]
        public string Name { get; set; }
        public int LocationId { get; set; }
        public IEnumerable<LocationWithIdDTO> Locations { get; set; } = new List<LocationWithIdDTO>();
    }
}
