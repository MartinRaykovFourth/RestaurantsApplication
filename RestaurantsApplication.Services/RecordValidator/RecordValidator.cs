using RestaurantsApplication.DTOs.DatabaseCopiesDTOs;
using static RestaurantsApplication.Services.Constants.FailMessages;

namespace RestaurantsApplication.Services.RecordValidator
{
    public class RecordValidator : IRecordValidator
    {
        public void ValidateRecords(RequestCopyDTO req, ref bool isFailed, Dictionary<string, int> errorsDictionary)
        {
            foreach (var rec in req.Records)
            {
                var employee = rec.Employee;

                if (!employee.Employments.Any(e =>
                e.Department.Location.Code == req.LocationCode
                && e.StartDate.Date <= req.Date
                && (e.EndDate.HasValue ? e.EndDate.Value.Date >= req.Date : true)))
                {
                    isFailed = true;

                    var error = string.Format(EmployeeHasNoEmploymentInLocation, employee.Code, req.Location.Name);
                    ManageError(errorsDictionary, error);
                }

                if ((int)rec.ClockStatus < 0 || (int)rec.ClockStatus > 3)
                {
                    isFailed = true;

                    var error = string.Format(ClockStatusNotValid, employee.Code, (int)rec.ClockStatus);
                    ManageError(errorsDictionary, error);
                }

                if (rec.ClockValue.Date > req.Date.Date)
                {
                    isFailed = true;

                    var error = string.Format(ClockValueNotValid, employee.Code);
                    ManageError(errorsDictionary, error);
                }
            }
        }

        private void ManageError(Dictionary<string, int> errorsDictionary, string error)
        {
            if (errorsDictionary.ContainsKey(error))
            {
                errorsDictionary[error]++;
            }
            else
            {
                errorsDictionary[error] = 1;
            }
        }
    }
}
