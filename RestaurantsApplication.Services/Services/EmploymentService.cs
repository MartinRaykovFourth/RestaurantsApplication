using RestaurantsApplication.Data;
using RestaurantsApplication.Data.Entities;
using RestaurantsApplication.DTOs.EmploymentDTOs;
using RestaurantsApplication.Services.Contracts;

namespace RestaurantsApplication.Services.Services
{
    public class EmploymentService : IEmploymentService
    {
        private readonly RestaurantsContext _context;

        public EmploymentService(RestaurantsContext context)
        {
            _context = context;
        }

        public async Task AddAsync(EmploymentShortInfoDTO dto)
        {
            var employment = new Employment()
            {
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                DepartmentId = dto.DepartmentId,
                EmployeeId = dto.EmployeeId,
                RoleId = dto.RoleId,
                IsMain = dto.IsMain,
                Rate = dto.Rate
            };

            _context.Employments.Add(employment);
            await _context.SaveChangesAsync();
        }
    }
}
