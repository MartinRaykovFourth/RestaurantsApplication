namespace RestaurantsApplication.Services.Contracts
{
    public interface IValidatorService
    {
        public Task<bool> ValidateDepartmentAsync(int departmentId, int locationId);
        public Task<int> ValidateEmployeeAsync(string employeeCode);
        public Task<bool> CanEmploymentBeMainAsync(int employeeId);
    }
}