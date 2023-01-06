using System.ComponentModel.DataAnnotations;
using static RestaurantsApplication.MVC.Messages.ErrorMessages;

namespace RestaurantsApplication.MVC.Models.Location
{
    public class LocationShortInfoViewModel
    {
        [Required(ErrorMessage = LocationNameRequiredError)]
        [MaxLength(50)]
        public string Name { get; set; }

        [Required(ErrorMessage = CodeRequiredError)]
        [StringLength(8, MinimumLength = 8,ErrorMessage = CodeError)]
        public string Code { get; set; }
    }
}
