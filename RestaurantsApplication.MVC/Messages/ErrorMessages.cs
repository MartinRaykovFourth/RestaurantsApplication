namespace RestaurantsApplication.MVC.Messages
{
    public class ErrorMessages
    {
        public const string EmployeeFirstNameRequiredError = "First name is required";
        public const string EmployeeLastNameRequiredError = "Last name is required";
        public const string EmployeeWrongCodeError = "Employee with this code doesn't exist!";
        public const string EmployeeAlreadyHasMainEmploymentError = "This employee already has a main employment!";
        public const string EmployeeAlreadyHasEmploymentWithRole = "This employee already has an employment with that role in that department!";

        public const string LocationNameRequiredError = "Location name is required";

        public const string DepartmentNameRequiredError = "Department name is required";
        public const string DepartmentDoesntExistInLocationError = "The chosen location doesn't have this department!";

        public const string CodeError = "The code must be exactly 8 symbols long!";
        public const string CodeRequiredError = "A code is required";

        public const string EarlierEndDateMessage = "The end date of the employment can not be earlier than the start date!";
        public const string CantDeleteMainEmployment = "To delete this employment first choose another employment as main!";

        public const string StartDateRequiredError = "Start date is required!";

    }
}
