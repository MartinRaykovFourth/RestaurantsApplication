﻿using RestaurantsApplication.Data.Entities;

namespace RestaurantsApplication.DTOs.ShiftDTOs
{
    public class ShiftWithEmployeeDTO
    {
        public string EmployeeName { get; set; }
        public DateTime Start { get; set;}
        public DateTime End { get; set;}
        public DateTime? BreakStart { get; set; }
        public DateTime? BreakEnd { get; set; }
        public TimeSpan HoursWorked { get; set; }
        public decimal? Cost { get; set; }
        public int? RoleId { get; set; }
        public int? DepartmentId { get; set; }
        public Employee Employee { get; set; }
    }
}
