using Microsoft.AspNetCore.Mvc;
using RestaurantsApplication.DTOs.RoleDTOs;
using System.ComponentModel.DataAnnotations;

namespace RestaurantsApplication.MVC.Models.Shift
{
    public class OverlappedShiftViewModel
    {
        public int ShiftId { get; set; }
        public string EmployeeName { get; set; } = null!;
        public string EmployeeCode { get; set; } = null!;
        [Required]
        public DateTime? Start { get; set; }
        [Required]
        public DateTime? End { get; set; }
        public DateTime? BreakStart { get; set; }
        public DateTime? BreakEnd { get; set; }
        public int? RoleId { get; set; }
        public bool ForRemoval { get; set; }
        [BindProperty]
        public List<RoleWithIdDTO> AvailableRoles { get; set; } = new List<RoleWithIdDTO>();
    }
}
