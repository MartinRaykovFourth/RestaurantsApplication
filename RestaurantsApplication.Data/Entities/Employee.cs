using System.ComponentModel.DataAnnotations;

namespace RestaurantsApplication.Data.Entities
{
    public class Employee
    {
        public int Id { get; set; }
        [MaxLength(20)]
        public string FirstName { get; set; }

        [MaxLength(20)]
        public string LastName { get; set; }

        [MaxLength(8)]
        public string Code { get; set; }
    }
}
