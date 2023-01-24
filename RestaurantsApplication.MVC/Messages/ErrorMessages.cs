namespace RestaurantsApplication.MVC.Messages
{
    public class ErrorMessages
    {
        public const string EmployeeFirstNameRequiredError = "First name is required";
        public const string EmployeeLastNameRequiredError = "Last name is required";
        public const string EmployeeWrongCodeError = "Employee with this code doesn't exist!";
        public const string EmployeeAlreadyHasMainEmploymentError = "This employee already has a main employment!";
        public const string EmployeeAlreadyHasEmploymentWithRoleError = "This employee already has an employment with that role in that department!";

        public const string LocationNameRequiredError = "Location name is required";

        public const string DepartmentNameRequiredError = "Department name is required";
        public const string DepartmentDoesntExistInLocationError = "The chosen location doesn't have this department!";

        public const string CodeError = "The code must be exactly 8 symbols long!";
        public const string CodeRequiredError = "A code is required";

        public const string EarlierEndDateError = "The end date of the employment can not be earlier than the start date!";
        public const string CantDeleteMainEmploymentError = "To delete this employment first choose another employment as main!";

        public const string StartDateRequiredError = "Start date is required!";

        public const string InvalidClockTimeError = "Invalid clock value!";
        public const string BreakStartWithoutEndError = "Can't have only break start!";
        public const string BreakEndWithoutStartError = "Can't have only break end!";

        public const string ShiftsStillOverlappedError = "There are shifts that are still overlapped!";

        public const string DateChangedError = "Can't change the dates of the shifts!";
    }
}
