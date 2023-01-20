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

        private void ValidateRecords(Request req, ref bool isFailed, Dictionary<string, int> errorsDictionary)
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

                if (rec.ClockValue.Date > req.Date.Date)
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
            foreach (var rec in req.Records) // suzdavame shiftove v lista ot vseki edin record
            {
                switch (rec.ClockStatus)
                {
                    case ClockStatus.ClockIn:
                        await ManageClockStatusOrCreateShift(req, rec, shifts, rec.ClockStatus);
                        break;

                    case ClockStatus.BreakStart:
                        await ManageClockStatusOrCreateShift(req, rec, shifts, rec.ClockStatus);
                        break;

                    case ClockStatus.BreakEnd:
                        await ManageClockStatusOrCreateShift(req, rec, shifts, rec.ClockStatus);
                        break;

                    case ClockStatus.ClockOut:
                        await ManageClockStatusOrCreateShift(req, rec, shifts, rec.ClockStatus);
                        break;
                }
            }

            for (int i = 0; i < shifts.Count; i++) // proverqvame shift lista za doubled shifts i gi chistim
            {
                var currentShift = shifts[i]; // moje da si prost i da e null 

                var doubledShifts = shifts.Count(s =>
                        s.Employee.Code == currentShift.Employee.Code
                        && s.Start.HasValue && currentShift.Start.HasValue && s.Start == currentShift.Start
                        && s.End.HasValue && currentShift.End.HasValue && s.End == currentShift.End
                        &&
                        (
                        (s.BreakStart.HasValue && currentShift.BreakStart.HasValue && s.BreakStart == currentShift.BreakStart)
                        || (s.BreakStart.HasValue == false && currentShift.BreakStart.HasValue == false)
                        )
                        &&
                        (
                        (s.BreakEnd.HasValue && currentShift.BreakEnd.HasValue && s.BreakEnd == currentShift.BreakEnd)
                        || (s.BreakEnd.HasValue == false && currentShift.BreakEnd.HasValue == false)
                        ));

                if (doubledShifts > 1)
                {
                    shifts.RemoveAll(s =>
                        s.Employee.Code == currentShift.Employee.Code
                        && s.Start.HasValue && currentShift.Start.HasValue && s.Start == currentShift.Start
                        && s.End.HasValue && currentShift.End.HasValue && s.End == currentShift.End
                        &&
                        (
                        (s.BreakStart.HasValue && currentShift.BreakStart.HasValue && s.BreakStart == currentShift.BreakStart)
                        || (s.BreakStart.HasValue == false && currentShift.BreakStart.HasValue == false)
                        )
                        &&
                        (
                        (s.BreakEnd.HasValue && currentShift.BreakEnd.HasValue && s.BreakEnd == currentShift.BreakEnd)
                        || (s.BreakEnd.HasValue == false && currentShift.BreakEnd.HasValue == false)
                        ));

                    shifts.Add(currentShift);
                }
            }

            shifts.RemoveAll(sh =>                                              //proverqvame bazata za dobuled i gi chistim ot lista
                _context.Shifts.Any(s =>
                s.Employee.Code == sh.Employee.Code
                && s.Start.HasValue && sh.Start.HasValue && s.Start == sh.Start
                && s.End.HasValue && sh.End.HasValue && s.End == sh.End
                &&
                (
                (s.BreakStart.HasValue && sh.BreakStart.HasValue && s.BreakStart == sh.BreakStart)
                || (s.BreakStart.HasValue == false && sh.BreakStart.HasValue == false)
                )
                &&
                (
                (s.BreakEnd.HasValue && sh.BreakEnd.HasValue && s.BreakEnd == sh.BreakEnd)
                || (s.BreakEnd.HasValue == false && sh.BreakEnd.HasValue == false)
                )
                ));

            for (int i = 0; i < shifts.Count; i++) // napasvame brakeove za shiftove ot predni requesti i triem nevalidni shiftove
            {
                var currentShift = shifts[i];

                if (!currentShift.Start.HasValue && !currentShift.End.HasValue)
                {
                    if (currentShift.BreakStart.HasValue)
                    {
                        var shiftFromBase = await _context.Shifts
                            .Where(s =>
                            s.Employee.Code == currentShift.Employee.Code
                            && s.BreakStart.HasValue == false
                            && s.Start <= currentShift.BreakStart
                            && s.End > currentShift.BreakStart)
                            .FirstOrDefaultAsync();

                        if (shiftFromBase != null)
                            shiftFromBase.BreakStart = currentShift.BreakStart;
                    }

                    if (currentShift.BreakEnd.HasValue)
                    {
                        var shiftFromBase = await _context.Shifts
                            .Where(s =>
                            s.Employee.Code == currentShift.Employee.Code
                            && s.BreakEnd.HasValue == false
                            && s.Start < currentShift.BreakEnd
                            && s.End >= currentShift.BreakEnd)
                            .FirstOrDefaultAsync();

                        if (shiftFromBase != null)
                            shiftFromBase.BreakEnd = currentShift.BreakEnd;
                    }

                    shifts.RemoveAt(i);
                    i--;
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

        private async Task ManageClockStatusOrCreateShift(Request req, Record rec, List<Shift> shifts, ClockStatus clockStatus)
        {
            Shift existingShift = null;

            switch (clockStatus)
            {
                case ClockStatus.ClockIn:
                    existingShift = shifts
                            .Where(s =>
                            s.Employee.Code == rec.EmployeeCode
                            && s.Start.HasValue == false
                            && (
                            (s.End.HasValue && s.End.Value.Date == req.Date && s.End > rec.ClockValue)
                            || (s.BreakStart.HasValue && s.BreakStart.Value.Date == req.Date && s.BreakStart >= rec.ClockValue)
                            || (s.BreakEnd.HasValue && s.BreakEnd.Value.Date == req.Date && s.BreakEnd > rec.ClockValue)
                            ))
                            .FirstOrDefault();

                    if (existingShift != null)
                    {
                        existingShift.Start = rec.ClockValue;
                        return;
                    }
                    break;

                case ClockStatus.ClockOut:
                    existingShift = shifts
                            .Where(s =>
                            s.Employee.Code == rec.EmployeeCode
                            && s.End.HasValue == false
                            && (
                            (s.Start.HasValue && s.Start.Value.Date == req.Date && s.Start < rec.ClockValue)
                            || (s.BreakStart.HasValue && s.BreakStart.Value.Date == req.Date && s.BreakStart < rec.ClockValue)
                            || (s.BreakEnd.HasValue && s.BreakEnd.Value.Date == req.Date && s.BreakEnd <= rec.ClockValue)
                            ))
                            .FirstOrDefault();

                    if (existingShift != null)
                    {
                        existingShift.End = rec.ClockValue;
                        return;
                    }
                    break;

                case ClockStatus.BreakStart:
                    existingShift = shifts
                            .Where(s =>
                            s.Employee.Code == rec.EmployeeCode
                            && s.BreakStart.HasValue == false
                            && (
                            (s.Start.HasValue && s.Start.Value.Date == req.Date && s.Start <= rec.ClockValue)
                            || (s.BreakEnd.HasValue && s.BreakEnd.Value.Date == req.Date && s.BreakEnd > rec.ClockValue)
                            || (s.End.HasValue && s.End.Value.Date == req.Date && s.End > rec.ClockValue)
                            ))
                            .FirstOrDefault();

                    if (existingShift != null)
                    {
                        existingShift.BreakStart = rec.ClockValue;
                        return;
                    }
                    break;

                case ClockStatus.BreakEnd:
                    existingShift = shifts
                            .Where(s =>
                            s.Employee.Code == rec.EmployeeCode
                            && s.BreakEnd.HasValue == false
                            && (
                            (s.Start.HasValue && s.Start.Value.Date == req.Date && s.Start < rec.ClockValue)
                            || (s.BreakStart.HasValue && s.BreakStart.Value.Date == req.Date && s.BreakStart < rec.ClockValue)
                            || (s.End.HasValue && s.End.Value.Date == req.Date && s.End > rec.ClockValue)
                            ))
                            .FirstOrDefault();

                    if (existingShift != null)
                    {
                        existingShift.BreakEnd = rec.ClockValue;
                        return;
                    }
                    break;
            }

            existingShift = await CreateNewShift(req, rec);
            shifts.Add(existingShift);
        }
    }
}
