using RestaurantsApplication.Data.Enums;
using RestaurantsApplication.DTOs.DatabaseCopiesDTOs;
using RestaurantsApplication.Repositories.Contracts;
using RestaurantsApplication.Services.Contracts;
using RestaurantsApplication.Services.RecordValidator;
using System.Text;
using static RestaurantsApplication.Services.Constants.FailMessages;
using static RestaurantsApplication.Services.Constants.RequestStatuses;

namespace RestaurantsApplication.Services.Services
{
    public class TNAService : ITNAService
    {
        private readonly IRequestRepository _requestRepository;
        private readonly IShiftRepository _shiftRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IRecordValidator _recordValidator;

        public TNAService(IRequestRepository requestRepository,
            IShiftRepository shiftRepository,
            IEmployeeRepository employeeRepository,
            IRecordValidator recordValidator)
        {
            _requestRepository = requestRepository;
            _shiftRepository = shiftRepository;
            _employeeRepository = employeeRepository;
            _recordValidator = recordValidator;
        }

        public async Task ProcessRequestsAsync()
        {
            if (_requestRepository.PendingRequestsExist())
            {
                List<RequestCopyDTO> requests = await _requestRepository.GetRequestsAndSetStatus();

                await _requestRepository.SaveChangesAsync();

                foreach (var req in requests)
                {
                    bool isFailed = false;
                    var errorsDictionary = new Dictionary<string, int>();

                    if (req.Date > DateTime.Now.Date)
                    {
                        isFailed = true;
                        errorsDictionary[DateIsLaterThanToday] = 1;
                    }

                    _recordValidator.ValidateRecords(req, ref isFailed, errorsDictionary);

                    if (isFailed)
                    {
                        req.Status = Failed;

                        await _requestRepository.ChangeRequestStatusAsync(req.Id, req.Status);
                        SetFailMessage(req, errorsDictionary);

                        await _requestRepository.SaveChangesAsync();

                        continue;
                    }

                    var shifts = new List<ShiftCopyDTO>();
                    await ManageShifts(req, shifts);

                    req.Status = Completed;

                    await _requestRepository.ChangeRequestStatusAsync(req.Id, req.Status);

                    _shiftRepository.AddRange(shifts);

                    await _shiftRepository.SaveChangesAsync();
                }
            }
        }

        private void SetFailMessage(RequestCopyDTO req, Dictionary<string, int> errorsDictionary)
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

            _requestRepository.SetFailMessageAsync(req.Id, sb.ToString());
        }

        private async Task ManageShifts(RequestCopyDTO req, List<ShiftCopyDTO> shifts)
        {
            await ProcessRecords(req, shifts);

            RemoveDoubledShiftsFromList(shifts);

            shifts.RemoveAll(s => _shiftRepository.CheckEqualShiftsAsync(s));

            await ManageBreaksForExistingShifts(shifts);
        }

        private async Task ProcessRecords(RequestCopyDTO req, List<ShiftCopyDTO> shifts)
        {
            foreach (var rec in req.Records)
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
        }

        private void RemoveDoubledShiftsFromList(List<ShiftCopyDTO> shifts)
        {
            for (int i = 0; i < shifts.Count; i++)
            {
                var currentShift = shifts[i];

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
        }

        private async Task ManageBreaksForExistingShifts(List<ShiftCopyDTO> shifts)
        {
            for (int i = 0; i < shifts.Count; i++)
            {
                var currentShift = shifts[i];

                if (!currentShift.Start.HasValue && !currentShift.End.HasValue)
                {
                    if (currentShift.BreakStart.HasValue)
                    {
                        var shiftFromBase = await _shiftRepository.GetShiftWithoutBreakStartAsync(currentShift.Employee.Code, currentShift.BreakStart);

                        if (shiftFromBase != null)
                        {
                            shiftFromBase.BreakStart = currentShift.BreakStart;

                            await _shiftRepository.EditShiftClocksAsync(shiftFromBase);
                        }
                    }

                    if (currentShift.BreakEnd.HasValue)
                    {
                        var shiftFromBase = await _shiftRepository.GetShiftWithoutBreakEndAsync(currentShift.Employee.Code, currentShift.BreakEnd);

                        if (shiftFromBase != null)
                        {
                            shiftFromBase.BreakEnd = currentShift.BreakEnd;

                            await _shiftRepository.EditShiftClocksAsync(shiftFromBase);
                        }
                    }

                    shifts.RemoveAt(i);
                    i--;
                }
            }
        }

        private async Task ManageClockStatusOrCreateShift(RequestCopyDTO req, RecordCopyDTO rec, List<ShiftCopyDTO> shifts, ClockStatus clockStatus)
        {
            ShiftCopyDTO existingShift = null;

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

        private async Task<ShiftCopyDTO> CreateNewShift(RequestCopyDTO req, RecordCopyDTO rec)
        {
            var newShift = new ShiftCopyDTO();

            var employee = await _employeeRepository.GetByCodeWithIncludesAsync(rec.EmployeeCode);

            newShift.EmployeeId = employee.Id;
            newShift.Employee = employee;

            try
            {
                var employment = employee.Employments
                     .Where(e => e.Department.Location.Code == req.LocationCode)
                     .SingleOrDefault();

                if (employment != null)
                {
                    newShift.DepartmentId = employment.DepartmentId;
                    newShift.RoleId = employment.RoleId;
                }
            }
            catch (Exception)
            {

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
