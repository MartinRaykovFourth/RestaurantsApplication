using Microsoft.EntityFrameworkCore;
using RestaurantsApplication.Data;
using RestaurantsApplication.Data.Entities;
using RestaurantsApplication.DTOs.DatabaseCopiesDTOs;
using RestaurantsApplication.Repositories.Contracts;

namespace RestaurantsApplication.Repositories
{
    public class RequestRepository : IRequestRepository
    {
        private readonly RestaurantsContext _context;

        public RequestRepository(RestaurantsContext context)
        {
            _context = context;
        }

        public bool PendingRequestsExist()
        {
            return _context.Requests.Any(r => r.Status == "Pending");
        }

        public void Add(RequestCopyDTO requestDTO, List<RecordCopyDTO> recordDTOs)
        {
            var request = new Request()
            {
                Date = requestDTO.Date,
                LocationCode = requestDTO.LocationCode,
                Status = requestDTO.Status
            };

            var records = recordDTOs
                .Select(r => new Record
                {
                    Request = request,
                    ClockStatus = r.ClockStatus,
                    ClockValue = r.ClockValue,
                    EmployeeCode = r.EmployeeCode
                })
                .ToList();

            _context.Requests.Add(request);
            _context.Records.AddRange(records);
        }

        public async Task<List<RequestCopyDTO>> GetRequestsAndSetStatus()
        {
            var requests = await _context.Requests
                .Where(r => r.Status == "Pending")
                .Include(r => r.Records)
                .ThenInclude(r => r.Employee)
                .ThenInclude(e => e.Employments)
                .ThenInclude(e => e.Role)
                .Include(r => r.Records)
                .ThenInclude(r => r.Employee)
                .ThenInclude(e => e.Employments)
                .ThenInclude(e => e.Department)
                .ThenInclude(d => d.Location)
                .Include(r => r.Location)
                .ToListAsync();

            foreach (var req in requests)
            {
                req.Status = "Processing";
            }

            List<RequestCopyDTO> dtos = requests
                .Select(r => new RequestCopyDTO
                {
                    Id = r.Id,
                    Date = r.Date,
                    LocationCode = r.LocationCode,
                    Status = "Processing",
                    Location = new LocationCopyDTO
                    {
                        Id = r.Location.Id,
                        Code = r.Location.Code,
                        Name = r.Location.Name
                    },
                    Records = r.Records
                    .Select(rec => new RecordCopyDTO
                    {
                        Id = rec.Id,
                        ClockStatus = rec.ClockStatus,
                        ClockValue = rec.ClockValue,
                        EmployeeCode = rec.EmployeeCode,
                        RequestId = r.Id,
                        Employee = new EmployeeCopyDTO
                        {
                            Id = rec.Employee.Id,
                            Code = rec.EmployeeCode,
                            FirstName = rec.Employee.FirstName,
                            LastName = rec.Employee.LastName,
                            Employments = rec.Employee.Employments
                            .Select(e => new EmploymentCopyDTO
                            {
                                Id = e.Id,
                                DepartmentId = e.DepartmentId,
                                EmployeeId = e.Employee.Id,
                                StartDate = e.StartDate,
                                EndDate = e.EndDate,
                                IsMain = e.IsMain,
                                Rate = e.Rate,
                                RoleId = e.RoleId,
                                Role = new RoleCopyDTO
                                {
                                    Id = e.Role.Id,
                                    Name = e.Role.Name
                                },
                                Department = new DepartmentCopyDTO
                                {
                                    Id = e.Department.Id,
                                    LocationId = e.Department.Location.Id,
                                    Name = e.Department.Name,
                                    Location = new LocationCopyDTO
                                    {
                                        Id = e.Department.Location.Id,
                                        Code = e.Department.Location.Code,
                                        Name = e.Department.Location.Name
                                    }
                                }
                            })
                            .ToList()
                        }
                    })
                    .ToList()
                })
                .ToList();

            return dtos;
        }

        public async Task ChangeRequestStatusAsync(int requestId, string status)
        {
            var request = await _context.Requests.FindAsync(requestId);

            request.Status = status;
        }

        public async Task SetFailMessageAsync(int requestId, string failMessage)
        {
            var request = await _context.Requests.FindAsync(requestId);

            request.FailMessage = failMessage;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }


    }
}
