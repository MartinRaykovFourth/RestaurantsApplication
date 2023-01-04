using RestaurantsApplication.DTOs.EmployeeDTOs;

namespace RestaurantsApplication.Services.Contracts
{
    public interface IEmployeeService
    {
        public Task AddEmployee(EmployeeShortInfoDTO dto);
    }
}
