using System.ComponentModel.DataAnnotations.Schema;

namespace RestaurantsApplication.Data.Entities
{
    public class Employment
    {
        public int Id { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public int DepartmentId { get; set; }
        [ForeignKey(nameof(DepartmentId))]
        public Department Department { get; set; }

        public int RoleId { get; set; }
        [ForeignKey(nameof(RoleId))]
        public Role Role { get; set; }

        public int EmployeeId { get; set; }
        [ForeignKey(nameof(EmployeeId))]
        public Employee Employee { get; set; }

        public bool IsMain { get; set; }

        public decimal Rate { get; set; }
        public bool IsDeleted { get; set; }

    }
}
