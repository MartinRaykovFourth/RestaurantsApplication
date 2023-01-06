using System.ComponentModel.DataAnnotations;
using static RestaurantsApplication.MVC.Messages.ErrorMessages;

namespace RestaurantsApplication.MVC.Models.Employee
{
    public class EmployeeShortInfoViewModel
    {
        [Required(ErrorMessage = EmployeeFirstNameRequiredError)]
        [MaxLength(20)]
        public string FirstName { get; set; }

        [Required(ErrorMessage = EmployeeLastNameRequiredError)]
        [MaxLength(20)]
        public string LastName { get; set; }

        [Required(ErrorMessage = CodeRequiredError)]
        [StringLength(8, MinimumLength =8, ErrorMessage = CodeError)]
        public string Code { get; set; }
    }
}
