using System.ComponentModel.DataAnnotations;
using static RestaurantsApplication.MVC.ErrorMessages;

namespace RestaurantsApplication.MVC.Models.Employee
{
    public class EmployeeShortInfoViewModel
    {
        [Required(ErrorMessage = EmployeeFirstNameError)]
        [MaxLength(20)]
        public string FirstName { get; set; }

        [Required(ErrorMessage = EmployeeLastNameError)]
        [MaxLength(20)]
        public string LastName { get; set; }

        [StringLength(8, MinimumLength =8, ErrorMessage = CodeError)]
        public string Code { get; set; }
    }
}
