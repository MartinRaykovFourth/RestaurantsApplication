using Microsoft.EntityFrameworkCore;
using RestaurantsApplication.Data;
using RestaurantsApplication.Data.Entities;
using RestaurantsApplication.Data.Enums;
using System.Text;
using static TNAShiftCreatorService.Constants.FailMessages;
using static TNAShiftCreatorService.Constants.RequestStatuses;

namespace TNAShiftCreatorService
{
    public class Worker : BackgroundService
    {
        private readonly RestaurantsContext _context;

        public Worker(IServiceProvider serviceProvider)
        {
            _context = serviceProvider.CreateScope().ServiceProvider.GetRequiredService<RestaurantsContext>();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (_context.Requests.Any(r => r.Status == Pending))
                {
                    List<Request> requests = await GetRequests();

                    foreach (var req in requests)
                    {
                        bool isFailed = false;
                        var errorsDictionary = new Dictionary<string, int>();

                        if (req.Date > DateTime.Now.Date)
                        {
                            isFailed = true;
                            errorsDictionary[DateIsLaterThanToday] = 1;
                        }

                        ValidateRecords(req, ref isFailed, errorsDictionary);

                        if (isFailed)
                        {
                            req.Status = Failed;
                            SetFailMessage(req, errorsDictionary);

                            await _context.SaveChangesAsync();

                            continue;
                        }

                        var shifts = new List<Shift>();
                        await ManageShifts(req, shifts);

                        req.Status = Completed;

                        _context.Shifts.AddRange(shifts);
                        await _context.SaveChangesAsync();
                    }
                }

                Thread.Sleep(60 * 1000);
            }
        }

        private async Task<List<Request>> GetRequests()
        {
            var requests = await _context.Requests
                .Where(r => r.Status == Pending)
                .Include(r => r.Records)
                .ThenInclude(r => r.Employee)
                .ThenInclude(e => e.Employments)
                .ThenInclude(e => e.Department)
                .ThenInclude(d => d.Location)
                .Include(r => r.Location)
                .ToListAsync();

            foreach (var req in requests)
            {
                req.Status = Processing;
            }
            await _context.SaveChangesAsync();

            return requests;
        }

        private void ValidateRecords(Request req, ref bool isFailed,Dictionary<string, int> errorsDictionary)
        {
            foreach (var rec in req.Records)
            {
                var employee = rec.Employee;

                if (!employee.Employments.Any(e =>
                e.Department.Location.Code == req.LocationCode
                && e.StartDate.Date <= req.Date
                && (e.EndDate.HasValue ? e.EndDate.Value.Date >= req.Date : true)
                && e.IsDeleted == false))
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

                if (rec.ClockValue.Date > DateTime.Now.Date)
                {
                    isFailed = true;

                    var error = string.Format(ClockValueNotValid, employee.Code);
                    ManageError(errorsDictionary, error);
                }
            }
        }

        private static void ManageError(Dictionary<string, int> errorsDictionary, string error)
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

        private void SetFailMessage(Request req, Dictionary<string, int> errorsDictionary)
        {
            var sb = new StringBuilder();

            foreach (var error in errorsDictionary)
            {
                if (error.Value > 1)
                {
                    sb.AppendLine(error.Key + $"({error.Value})");
                }
                else
                {
                    sb.AppendLine(error.Key);
                }
            }

            req.FailMessage = sb.ToString();
        }

        private async Task ManageShifts(Request req, List<Shift> shifts)
        {
            foreach (var rec in req.Records)
            {
                var shift = await _context.Shifts
                    .Where(s =>
                    s.Employee.Code == rec.EmployeeCode
                    && s.Start.Value.Date == req.Date)
                    .SingleOrDefaultAsync();

                if (shift != null)
                {
                    if (rec.ClockStatus == ClockStatus.BreakStart && !shift.BreakStart.HasValue)
                    {
                        shift.BreakStart = rec.ClockValue;
                    }
                    else if (rec.ClockStatus == ClockStatus.BreakEnd && !shift.BreakEnd.HasValue)
                    {
                        shift.BreakEnd = rec.ClockValue;
                    }
                }
                else if (shifts.Any(s => s.Employee.Code == rec.EmployeeCode))
                {
                    var shiftFromList = shifts
                        .Where(s => s.Employee.Code == rec.EmployeeCode)
                        .Single();

                    switch (rec.ClockStatus)
                    {
                        case ClockStatus.ClockIn:
                            if (!shiftFromList.Start.HasValue)
                                shiftFromList.Start = rec.ClockValue;
                            break;
                        case ClockStatus.ClockOut:
                            if (!shiftFromList.End.HasValue)
                                shiftFromList.End = rec.ClockValue;
                            break;
                        case ClockStatus.BreakStart:
                            if (!shiftFromList.BreakStart.HasValue)
                                shiftFromList.BreakStart = rec.ClockValue;
                            break;
                        case ClockStatus.BreakEnd:
                            if (!shiftFromList.BreakEnd.HasValue)
                                shiftFromList.BreakEnd = rec.ClockValue;
                            break;
                    }
                }
                else
                {
                    Shift newShift = await CreateNewShift(req, rec);
                    shifts.Add(newShift);
                }
            }
        }

        private async Task<Shift> CreateNewShift(Request req, Record rec)
        {
            var newShift = new Shift();

            var employee = await _context.Employees
                .Include(e => e.Employments)
                .ThenInclude(e => e.Department)
                .ThenInclude(d => d.Location)
                .Where(e => e.Code == rec.EmployeeCode)
                .SingleAsync();

            newShift.EmployeeId = employee.Id;
            newShift.Employee = employee;

            if (employee.Employments.Count(e =>
            e.Department.Location.Code == req.LocationCode
            && e.IsDeleted == false) == 1)
            {
                var employment = employee.Employments
                     .Where(e =>
                     e.Department.Location.Code == req.LocationCode
                     && e.IsDeleted == false)
                     .Single();

                newShift.DepartmentId = employment.DepartmentId;
                newShift.RoleId = employment.RoleId;
            }

            switch (rec.ClockStatus)
            {
                case ClockStatus.ClockIn:
                    newShift.Start = rec.ClockValue;
                    break;
                case ClockStatus.ClockOut:
                    newShift.End = rec.ClockValue;
                    break;
                case ClockStatus.BreakStart:
                    newShift.BreakStart = rec.ClockValue;
                    break;
                case ClockStatus.BreakEnd:
                    newShift.BreakEnd = rec.ClockValue;
                    break;
            }

            return newShift;
        }

    }
}