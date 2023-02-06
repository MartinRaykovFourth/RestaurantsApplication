namespace RestaurantsApplication.Services.Constants
{
    public class FailMessages
    {
        public const string EmployeeHasNoEmploymentInLocation = "The employee with code {0} doesn't have an active employment in {1} location! ";

        public const string ClockStatusNotValid = "The employee with code {0} has invalid clock status - {1}! ";

        public const string DateIsLaterThanToday = "The date of the records can not be later than today! ";

        public const string ClockValueNotValid = "Employee with code {0} has invalid clock value! ";
    }
}
