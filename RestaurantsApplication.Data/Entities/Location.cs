using System.ComponentModel.DataAnnotations;

namespace RestaurantsApplication.Data.Entities
{
    public class Location
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(50)]
        public string Name { get; set; }

        [StringLength(8, MinimumLength = 8)]
        public string Code { get; set;}

        public List<Department> Departments { get; set; } = new List<Department>();
        public bool IsDeleted { get; set; }
    }
}