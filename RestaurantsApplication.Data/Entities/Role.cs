using System.ComponentModel.DataAnnotations;

namespace RestaurantsApplication.Data.Entities
{
    public class Role
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }
    }
}
